import styles from './CourseCardSkeleton.module.scss';

export const CourseCardSkeleton = () => (
  <article className={styles.card} aria-hidden>
    <span className={styles.cover} />
    <section className={styles.body}>
      <span className={styles.lineShort} />
      <span className={styles.lineTitle} />
      <span className={styles.line} />
      <span className={styles.line} />
    </section>
  </article>
);
