import { FormEvent, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { resetPassword } from '../../api/authExtras';
import { AuthScreen } from '../../components/AuthScreen/AuthScreen';
import { FormField } from '../../components/FormField/FormField';
import { toast } from 'react-toastify';
import styles from './ResetPasswordPage.module.scss';

export const ResetPasswordPage = () => {
  const navigate = useNavigate();
  const [token, setToken] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await resetPassword(token, password);
      toast.success('Пароль обновлён');
      navigate('/login');
    } catch {
      toast.error('Неверный или просроченный токен');
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthScreen title="Новый пароль">
      <form onSubmit={onSubmit} className={styles.form}>
        <FormField
          label="Токен из лога Seq"
          type="text"
          value={token}
          onChange={(e) => setToken(e.target.value)}
          required
        />
        <FormField
          label="Новый пароль"
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
          autoComplete="new-password"
        />
        <button type="submit" className={styles.submit} disabled={loading}>
          {loading ? 'Сохранение…' : 'Сохранить пароль'}
        </button>
      </form>
      <p className={styles.footer}>
        <Link to="/login">Вход</Link>
      </p>
    </AuthScreen>
  );
};
