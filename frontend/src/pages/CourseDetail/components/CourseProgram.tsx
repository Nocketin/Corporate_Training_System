import { Link } from 'react-router-dom';
import type { ModuleDto } from '../../../api/courses';
import styles from './CourseProgram.module.scss';

type Props = {
  courseId: string;
  modules: ModuleDto[];
  completedLessonIds: Set<string>;
  lockedLessonIds: Set<string>;
  isEnrolled: boolean;
};

export const CourseProgram = ({
  courseId,
  modules,
  completedLessonIds,
  lockedLessonIds,
  isEnrolled,
}: Props) => (
  <>
    <h2 className={styles.heading}>Программа</h2>
    {modules.map((m) => (
      <section key={m.id} className={styles.module}>
        <h3 className={styles.moduleTitle}>
          {m.order + 1}. {m.title}
        </h3>
        <ul className={styles.lessonList}>
          {[...m.lessons]
            .sort((a, b) => a.order - b.order)
            .map((l) => {
              const isDone = completedLessonIds.has(l.id);
              const isLocked = isEnrolled && lockedLessonIds.has(l.id);
              const canOpen = isEnrolled && !isLocked;
              const duration = l.durationMinutes > 0 ? ` — ${l.durationMinutes} мин` : '';

              return (
                <li key={l.id} className={styles.lessonItem}>
                  {canOpen || isDone ? (
                    <Link
                      className={`${styles.lessonLink} ${isDone ? styles.lessonLinkDone : ''}`}
                      to={`/courses/${courseId}/lessons/${l.id}`}
                    >
                      {isDone ? '✓ ' : ''}
                      {l.title}
                      {duration}
                    </Link>
                  ) : (
                    <span className={`${styles.lessonText} ${isLocked ? styles.lessonLocked : ''}`}>
                      {isLocked ? '🔒 ' : ''}
                      {l.title}
                      {duration}
                    </span>
                  )}
                </li>
              );
            })}
        </ul>
      </section>
    ))}
  </>
);
