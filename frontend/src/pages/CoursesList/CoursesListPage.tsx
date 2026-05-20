import { useEffect, useMemo, useState } from 'react';
import { fetchCourses, type CourseListItem } from '../../api/courses';
import { PageHeader } from '../../components/PageHeader/PageHeader';
import { CourseCard } from './components/CourseCard';
import { CourseCardSkeleton } from './components/CourseCardSkeleton';
import styles from './CoursesListPage.module.scss';

const SKELETON_COUNT = 6;

export const CoursesListPage = () => {
  const [courses, setCourses] = useState<CourseListItem[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [query, setQuery] = useState('');

  useEffect(() => {
    let cancelled = false;
    (async () => {
      try {
        const data = await fetchCourses();
        if (!cancelled) setCourses(data);
      } catch {
        if (!cancelled) setError('Не удалось загрузить курсы');
      }
    })();
    return () => {
      cancelled = true;
    };
  }, []);

  const filtered = useMemo(() => {
    if (!courses) return [];
    const q = query.trim().toLowerCase();
    if (!q) return courses;
    return courses.filter(
      (c) =>
        c.title.toLowerCase().includes(q) || c.description.toLowerCase().includes(q),
    );
  }, [courses, query]);

  return (
    <div className={styles.page}>
      <section className={styles.hero}>
        <h2 className={styles.heroTitle}>Развивайте команду с CTS Academy</h2>
        <p className={styles.heroText}>
          Технические курсы по .NET, фронтенду, Kafka и DevOps, а также программы по лидерству. Выберите
          курс, запишитесь и проходите уроки в удобном темпе.
        </p>
        <div className={styles.toolbar}>
          <input
            type="search"
            className={styles.search}
            placeholder="Поиск по названию или описанию…"
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            aria-label="Поиск курсов"
          />
          {courses && (
            <span className={styles.stats}>
              {filtered.length} из {courses.length} курсов
            </span>
          )}
        </div>
      </section>

      <PageHeader
        title="Каталог курсов"
        subtitle="Все доступные программы обучения в вашей организации"
      />

      {error && <p className={styles.error}>{error}</p>}

      {courses === null && !error && (
        <div className={styles.grid}>
          {Array.from({ length: SKELETON_COUNT }, (_, i) => (
            <CourseCardSkeleton key={i} />
          ))}
        </div>
      )}

      {courses && filtered.length === 0 && !error && (
        <p className={styles.empty}>
          {query ? 'По запросу ничего не найдено. Попробуйте другие слова.' : 'Курсы пока не добавлены.'}
        </p>
      )}

      {filtered.length > 0 && (
        <div className={styles.grid}>
          {filtered.map((c) => (
            <CourseCard key={c.id} course={c} />
          ))}
        </div>
      )}
    </div>
  );
};
