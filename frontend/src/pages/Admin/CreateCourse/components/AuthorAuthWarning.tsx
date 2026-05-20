import { Link } from 'react-router-dom';
import styles from './AuthorAuthWarning.module.scss';

export const AuthorAuthWarning = () => (
  <p className={styles.banner}>
    Вы не авторизованы. <Link to="/login">Войдите</Link>, чтобы получить AuthorId.
  </p>
);
