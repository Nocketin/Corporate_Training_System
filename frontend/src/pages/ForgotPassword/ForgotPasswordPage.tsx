import { FormEvent, useState } from 'react';
import { Link } from 'react-router-dom';
import { forgotPassword } from '../../api/authExtras';
import { AuthScreen } from '../../components/AuthScreen/AuthScreen';
import { FormField } from '../../components/FormField/FormField';
import { toast } from 'react-toastify';
import styles from './ForgotPasswordPage.module.scss';

export const ForgotPasswordPage = () => {
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [sent, setSent] = useState(false);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      await forgotPassword(email);
      setSent(true);
      toast.info('Если email зарегистрирован, ссылка для сброса записана в лог Seq');
    } catch {
      toast.error('Не удалось отправить запрос');
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthScreen title="Восстановление доступа">
      {sent ? (
        <p className={styles.message}>
          Запрос принят. В режиме разработки токен сброса пишется в лог Identity Service (Seq).
        </p>
      ) : (
        <form onSubmit={onSubmit} className={styles.form}>
          <FormField
            label="Email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            autoComplete="email"
          />
          <button type="submit" className={styles.submit} disabled={loading}>
            {loading ? 'Отправка…' : 'Отправить'}
          </button>
        </form>
      )}
      <p className={styles.footer}>
        <Link to="/login">Вернуться ко входу</Link>
        {' · '}
        <Link to="/reset-password">Уже есть токен?</Link>
      </p>
    </AuthScreen>
  );
};
