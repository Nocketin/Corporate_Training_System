import { Link } from 'react-router-dom';
import styles from './LoginFormFooter.module.scss';

export const LoginFormFooter = () => (
  <p className={styles.text}>
    <Link to="/forgot-password">Забыли пароль?</Link>
    {' · '}
    Нет аккаунта? <Link to="/register">Зарегистрироваться</Link>
  </p>
);
