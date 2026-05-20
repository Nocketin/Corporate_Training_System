import styles from './LessonLoading.module.scss';

export const LessonLoading = () => (
  <div className={styles.root} role="status" aria-label="Загрузка урока">
    <div className={styles.main}>
      <span className={`${styles.block} ${styles.title}`} />
      <span className={`${styles.block} ${styles.content}`} />
    </div>
    <div className={styles.sidebar}>
      <span className={`${styles.block} ${styles.sideItem}`} />
      <span className={`${styles.block} ${styles.sideItem}`} />
      <span className={`${styles.block} ${styles.sideItem}`} />
    </div>
  </div>
);
