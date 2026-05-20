import { Link } from 'react-router-dom';
import styles from './LessonNotFound.module.scss';

type Props = {
  courseId: string;
  message: string;
};

export const LessonNotFound = ({ courseId, message }: Props) => (
  <div className={styles.root}>
    <p className={styles.message}>{message}</p>
    <Link to={`/courses/${courseId}`} className={styles.link}>
      К курсу
    </Link>
  </div>
);
