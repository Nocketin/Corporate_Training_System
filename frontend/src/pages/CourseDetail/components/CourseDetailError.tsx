import { Link } from 'react-router-dom';
import styles from './CourseDetailError.module.scss';

type Props = {
  message: string | null;
};

export const CourseDetailError = ({ message }: Props) => (
  <div className={styles.root}>
    {message && <p className={styles.message}>{message}</p>}
    <Link to="/courses" className={styles.back}>
      Назад к витрине
    </Link>
  </div>
);
