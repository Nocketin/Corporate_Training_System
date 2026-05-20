import { FormEvent, useState } from 'react';
import { api } from '../../api/axiosInstance';

export const LoginPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [accessToken, setAccessToken] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const response = await api.post('/api/auth/login', {
        email,
        password,
      });

      setAccessToken(response.data.accessToken);
      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
    } catch (err: any) {
      setError(err.response?.data?.detail ?? 'Login failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
      <form
        onSubmit={handleSubmit}
        style={{
          width: 320,
          padding: 24,
          borderRadius: 8,
          boxShadow: '0 4px 20px rgba(0,0,0,0.08)',
          background: '#fff',
          fontFamily: 'system-ui, sans-serif',
        }}
      >
        <h2 style={{ marginBottom: 16 }}>Вход</h2>

        <label style={{ display: 'block', marginBottom: 8 }}>
          Email
          <input
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            style={{
              width: '100%',
              padding: '8px 10px',
              marginTop: 4,
              marginBottom: 12,
              borderRadius: 4,
              border: '1px solid #ccc',
            }}
          />
        </label>

        <label style={{ display: 'block', marginBottom: 8 }}>
          Пароль
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            style={{
              width: '100%',
              padding: '8px 10px',
              marginTop: 4,
              marginBottom: 12,
              borderRadius: 4,
              border: '1px solid #ccc',
            }}
          />
        </label>

        {error && (
          <div style={{ color: 'red', marginBottom: 12, fontSize: 14 }}>
            {error}
          </div>
        )}

        <button
          type="submit"
          disabled={loading}
          style={{
            width: '100%',
            padding: '10px 0',
            borderRadius: 4,
            border: 'none',
            background: '#2563eb',
            color: '#fff',
            fontWeight: 600,
            cursor: loading ? 'default' : 'pointer',
          }}
        >
          {loading ? 'Входим...' : 'Войти'}
        </button>

        {accessToken && (
          <p style={{ marginTop: 12, fontSize: 12, color: '#16a34a' }}>
            Успешный вход. AccessToken сохранён в localStorage.
          </p>
        )}

        <p style={{ marginTop: 12, fontSize: 14, textAlign: 'center' }}>
          Нет аккаунта?{' '}
          <a href="/register" style={{ color: '#2563eb', textDecoration: 'none' }}>
            Зарегистрироваться
          </a>
        </p>
      </form>
    </div>
  );
};

