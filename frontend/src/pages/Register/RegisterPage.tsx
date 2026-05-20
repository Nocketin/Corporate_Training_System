import { FormEvent, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import { api } from '../../api/axiosInstance';
import { setSession } from '../../lib/auth';
import { parseJwtRole, parseJwtSub } from '../../lib/jwt';
import { toast } from 'react-toastify';
import { AuthScreen } from '../../components/AuthScreen/AuthScreen';
import { FormField } from '../../components/FormField/FormField';
import { AuthErrorBanner } from '../../components/AuthErrorBanner/AuthErrorBanner';
import { RegisterFormFooter } from './components/RegisterFormFooter';
import styles from './RegisterPage.module.scss';

export const RegisterPage = () => {
  const navigate = useNavigate();
  const { refreshAuth } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      const response = await api.post('/api/auth/register', {
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
      toast.success('Регистрация успешна');
      navigate('/courses');
    } catch (err: unknown) {
      const ax = err as { response?: { data?: { detail?: string } } };
      setError(ax.response?.data?.detail ?? 'Registration failed');
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthScreen>
      <form className={styles.form} onSubmit={handleSubmit} noValidate>
        <h2 className={styles.title}>Регистрация</h2>

        <FormField
          id="register-email"
          label="Email"
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
          autoComplete="email"
        />

        <FormField
          id="register-password"
          label="Пароль"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          autoComplete="new-password"
        />

        {error && <AuthErrorBanner message={error} />}

        <button type="submit" className={styles.submit} disabled={loading}>
          {loading ? 'Регистрируем...' : 'Зарегистрироваться'}
        </button>

        <RegisterFormFooter />
      </form>
    </AuthScreen>
  );
};
