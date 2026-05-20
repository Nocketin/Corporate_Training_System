import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react-swc';

// base: './' in production so built assets resolve when served from disk or a subpath.
//
// Proxy (dev + preview): by default /api/auth → Identity, /api/courses → CourseService, other /api → Gateway
// (matches infra/docker-compose host ports 5000 / 5001 / 8080). No gateway required for auth + courses.
// Use 127.0.0.1 (not localhost) so Node connects via IPv4; on Windows+Docker, ::1 often refuses while the browser still works.
// Override: VITE_API_PROXY_TARGET=http://127.0.0.1:8080 to send all /api to one host.
// Fine-tune: VITE_PROXY_IDENTITY, VITE_PROXY_COURSES, VITE_PROXY_GATEWAY.
export default defineConfig(({ command, mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  const singleTarget = env.VITE_API_PROXY_TARGET;
  const identityTarget = env.VITE_PROXY_IDENTITY ?? 'http://127.0.0.1:5000';
  const coursesTarget = env.VITE_PROXY_COURSES ?? 'http://127.0.0.1:5001';
  const gatewayTarget = env.VITE_PROXY_GATEWAY ?? 'http://127.0.0.1:8080';
  const learningTarget = env.VITE_PROXY_LEARNING ?? gatewayTarget;

  const changeOrigin = true;
  const proxy = singleTarget
    ? {
        '/api': { target: singleTarget, changeOrigin },
      }
    : {
        '/api/auth': { target: identityTarget, changeOrigin },
        '/api/courses': { target: coursesTarget, changeOrigin },
        '/api/learning': { target: learningTarget, changeOrigin },
        '/api': { target: gatewayTarget, changeOrigin },
      };

  return {
    plugins: [react()],
    base: command === 'build' ? './' : '/',
    server: {
      port: Number(env.VITE_DEV_PORT) || 3000,
      strictPort: false,
      proxy,
    },
    preview: {
      port: Number(env.VITE_PREVIEW_PORT) || 3000,
      strictPort: false,
      proxy,
    },
  };
});

