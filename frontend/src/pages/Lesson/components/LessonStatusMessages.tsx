import styles from './LessonStatusMessages.module.scss';

type Props = {
  locked: boolean;
  completed: boolean;
  message: string | null;
};

export const LessonStatusMessages = ({ locked, completed, message }: Props) => (
  <>
    {locked && !completed && (
      <p className={styles.warning}>Сначала пройдите предыдущие уроки.</p>
    )}
    {completed && <p className={styles.success}>Урок пройден</p>}
    {message && <p className={styles.info}>{message}</p>}
  </>
);
