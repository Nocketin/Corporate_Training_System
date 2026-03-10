import { FormEvent, useState } from 'react';
import { api } from '../../api/axiosInstance';

export const RegisterPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(false);

    try {
      const response = await api.post('/api/auth/register', {
        email,
        password,
      });

      localStorage.setItem('accessToken', response.data.accessToken);
      localStorage.setItem('refreshToken', response.data.refreshToken);
      setSuccess(true);
    } catch (err: any) {
      setError(err.response?.data?.detail ?? 'Registration failed');
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
        <h2 style={{ marginBottom: 16 }}>Регистрация</h2>

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

        {success && (
          <div style={{ color: '#16a34a', marginBottom: 12, fontSize: 14 }}>
            Регистрация успешна! Теперь можете войти.
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
            background: '#16a34a',
            color: '#fff',
            fontWeight: 600,
            cursor: loading ? 'default' : 'pointer',
          }}
        >
          {loading ? 'Регистрируем...' : 'Зарегистрироваться'}
        </button>

        <p style={{ marginTop: 12, fontSize: 14, textAlign: 'center' }}>
          Уже есть аккаунт?{' '}
          <a href="/login" style={{ color: '#2563eb', textDecoration: 'none' }}>
            Войти
          </a>
        </p>
      </form>
    </div>
  );
};
