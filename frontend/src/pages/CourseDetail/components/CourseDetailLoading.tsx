import styles from './CourseDetailLoading.module.scss';

export const CourseDetailLoading = () => (
  <div className={styles.root} role="status" aria-label="Загрузка курса">
    <span className={styles.cover} />
    <span className={styles.title} />
    <span className={styles.line} />
    <span className={styles.lineShort} />
  </div>
);
