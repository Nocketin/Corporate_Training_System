import { FormEvent, useState } from 'react';
import { AxiosError } from 'axios';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { api } from '../../api/axiosInstance';
import { setSession } from '../../lib/auth';
import { parseJwtRole, parseJwtSub } from '../../lib/jwt';
import { toast } from 'react-toastify';
import { AuthScreen } from '../../components/AuthScreen/AuthScreen';
import { FormField } from '../../components/FormField/FormField';
import { AuthErrorBanner } from '../../components/AuthErrorBanner/AuthErrorBanner';
import { LoginFormFooter } from './components/LoginFormFooter';
import styles from './LoginPage.module.scss';

export const LoginPage = () => {
  const navigate = useNavigate();
  const { refreshAuth } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const resolveErrorMessage = (err: unknown): string => {
    if (!(err instanceof AxiosError)) {
      return 'Login failed';
    }

    const payload = err.response?.data;
    if (!payload || typeof payload !== 'object') {
      return 'Login failed';
    }

    if (typeof payload.detail === 'string' && payload.detail.length > 0) {
      return payload.detail;
    }

    if (payload.errors && typeof payload.errors === 'object') {
      const firstError = Object.values(payload.errors as Record<string, string[] | string>)
        .flatMap((entry) => (Array.isArray(entry) ? entry : [entry]))
        .find((message) => typeof message === 'string' && message.length > 0);

      if (firstError) {
        return firstError;
      }
    }

    return 'Login failed';
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const response = await api.post('/api/auth/login', {
        email,
        password,
      });

      const role =
        typeof response.data.role === 'string'
          ? response.data.role
          : parseJwtRole(response.data.accessToken);
      setSession(
        response.data.accessToken,
        response.data.refreshToken,
        parseJwtSub(response.data.accessToken),
        role,
      );
      refreshAuth();
      toast.success('Добро пожаловать!');
      navigate('/courses');
    } catch (err) {
      setError(resolveErrorMessage(err));
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthScreen>
      <form className={styles.form} onSubmit={handleSubmit} noValidate>
        <h2 className={styles.title}>Вход</h2>

        <FormField
          id="login-email"
          label="Email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          autoComplete="email"
        />

        <FormField
          id="login-password"
          label="Пароль"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          autoComplete="current-password"
        />

        {error && <AuthErrorBanner message={error} />}

        <button type="submit" className={styles.submit} disabled={loading}>
          {loading ? 'Входим...' : 'Войти'}
        </button>

        <LoginFormFooter />
      </form>
    </AuthScreen>
  );
};
