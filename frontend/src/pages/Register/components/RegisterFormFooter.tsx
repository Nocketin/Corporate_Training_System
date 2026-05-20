import { Link } from 'react-router-dom';
import styles from './RegisterFormFooter.module.scss';

export const RegisterFormFooter = () => (
  <p className={styles.text}>
    Уже есть аккаунт? <Link to="/login">Войти</Link>
  </p>
);
