import styles from './AnalyticsSkeleton.module.scss';

export const AnalyticsSkeleton = () => (
  <div className={styles.root} aria-hidden>
    <span className={styles.title} />
    <div className={styles.kpiRow}>
      <span className={styles.kpi} />
      <span className={styles.kpi} />
      <span className={styles.kpi} />
      <span className={styles.kpi} />
    </div>
    <span className={styles.chart} />
    <span className={styles.chart} />
  </div>
);
