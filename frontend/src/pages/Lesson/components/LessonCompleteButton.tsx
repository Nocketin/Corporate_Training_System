import styles from './LessonCompleteButton.module.scss';

type Props = {
  submitting: boolean;
  onClick: () => void;
};

export const LessonCompleteButton = ({ submitting, onClick }: Props) => (
  <button type="button" className={styles.button} onClick={onClick} disabled={submitting}>
    {submitting ? 'Сохранение…' : 'Завершить урок'}
  </button>
);
