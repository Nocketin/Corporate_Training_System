import type { CourseProgress } from '../../../api/learning';
import { CourseProgressBar } from './CourseProgressBar';
import styles from './CourseProgressSection.module.scss';

type Props = {
  progress: CourseProgress;
};

export const CourseProgressSection = ({ progress }: Props) => (
  <section className={styles.section} aria-labelledby="course-progress-heading">
    <div className={styles.row}>
      <span id="course-progress-heading">Прогресс</span>
      <span className={styles.stats}>
        {progress.completedLessons} / {progress.totalLessons} уроков
        {progress.status === 'Completed' ? ' — завершён' : ''}
      </span>
    </div>
    <CourseProgressBar
      percent={progress.progressPercent}
      isCompleted={progress.status === 'Completed'}
    />
  </section>
);
