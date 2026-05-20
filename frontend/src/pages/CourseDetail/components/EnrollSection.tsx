import styles from './EnrollSection.module.scss';

type Props = {
  enrolling: boolean;
  onEnroll: () => void;
  actionMessage: string | null;
};

export const EnrollSection = ({ enrolling, onEnroll, actionMessage }: Props) => (
  <div className={styles.wrap}>
    <button type="button" className={styles.button} onClick={onEnroll} disabled={enrolling}>
      {enrolling ? 'Запись…' : 'Записаться на курс'}
    </button>
    {actionMessage && <p className={styles.message}>{actionMessage}</p>}
  </div>
);
