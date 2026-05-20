import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { fetchMyEnrollments, type MyEnrollment } from '../../api/learning';
import { PageHeader } from '../../components/PageHeader/PageHeader';
import { useAuth } from '../../hooks/useAuth';
import styles from './MyLearningPage.module.scss';

const statusLabel: Record<string, string> = {
  Started: 'В процессе',
  Completed: 'Завершён',
};

export const MyLearningPage = () => {
  const { isLoggedIn } = useAuth();
  const [items, setItems] = useState<MyEnrollment[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!isLoggedIn) return;
    let cancelled = false;
    (async () => {
      try {
        const data = await fetchMyEnrollments();
        if (!cancelled) setItems(data);
      } catch {
        if (!cancelled) setError('Не удалось загрузить историю обучения');
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [isLoggedIn]);

  if (!isLoggedIn) {
    return (
      <div className={styles.page}>
        <PageHeader title="Моё обучение" />
        <p className={styles.hint}>
          <Link to="/login">Войдите</Link>, чтобы видеть записи на курсы.
        </p>
      </div>
    );
  }

  return (
    <div className={styles.page}>
      <PageHeader title="Моё обучение" subtitle="Ваши записи и прогресс" />

      {error && <p className={styles.error}>{error}</p>}

      {items === null && !error && <p className={styles.loading}>Загрузка…</p>}

      {items?.length === 0 && <p className={styles.empty}>Вы ещё не записаны ни на один курс.</p>}

      {items && items.length > 0 && (
        <ul className={styles.list}>
          {items.map((e) => (
            <li key={e.enrollmentId} className={styles.card}>
              <div className={styles.cardHead}>
                <Link to={`/courses/${e.courseId}`} className={styles.title}>
                  {e.courseTitle ?? 'Курс'}
                </Link>
                <span className={styles.badge}>{statusLabel[e.status] ?? e.status}</span>
              </div>
              <p className={styles.meta}>
                Начало: {new Date(e.startDate).toLocaleDateString('ru-RU')}
                {e.completeDate &&
                  ` · Завершение: ${new Date(e.completeDate).toLocaleDateString('ru-RU')}`}
              </p>
              {e.certificateFileUrl && (
                <p className={styles.cert}>Сертификат: {e.certificateFileUrl}</p>
              )}
              <Link to={`/courses/${e.courseId}`} className={styles.link}>
                Открыть курс →
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};
