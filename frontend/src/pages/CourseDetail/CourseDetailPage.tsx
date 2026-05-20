import { useCallback, useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { fetchCourse, type CourseDetail } from '../../api/courses';
import { baseURL } from '../../api/axiosInstance';
import {
  enrollInCourse,
  fetchCourseProgress,
  type CourseProgress,
} from '../../api/learning';
import { Breadcrumbs } from '../../components/Breadcrumbs/Breadcrumbs';
import { countLessonsFromDetail } from '../../lib/courseAccent';
import { CourseCover } from './components/CourseCover';
import { CourseDetailError } from './components/CourseDetailError';
import { CourseDetailLoading } from './components/CourseDetailLoading';
import { CourseProgram } from './components/CourseProgram';
import { CourseProgressSection } from './components/CourseProgressSection';
import { EnrollSection } from './components/EnrollSection';
import { toast } from 'react-toastify';
import styles from './CourseDetailPage.module.scss';

export const CourseDetailPage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [course, setCourse] = useState<CourseDetail | null>(undefined);
  const [progress, setProgress] = useState<CourseProgress | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [actionMessage, setActionMessage] = useState<string | null>(null);
  const [enrolling, setEnrolling] = useState(false);

  const loadProgress = useCallback(async (courseId: string) => {
    if (!localStorage.getItem('accessToken')) {
      setProgress(null);
      return;
    }
    try {
      const p = await fetchCourseProgress(courseId);
      setProgress(p);
    } catch {
      setProgress(null);
    }
  }, []);

  useEffect(() => {
    if (!id) return;
    let cancelled = false;
    (async () => {
      try {
        const data = await fetchCourse(id);
        if (!cancelled) {
          setCourse(data);
          await loadProgress(id);
        }
      } catch {
        if (!cancelled) {
          setError('Курс не найден');
          setCourse(null);
        }
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [id, loadProgress]);

  const onEnroll = async () => {
    if (!id) return;
    if (!localStorage.getItem('accessToken')) {
      navigate('/login');
      return;
    }
    setEnrolling(true);
    setActionMessage(null);
    try {
      await enrollInCourse(id);
      await loadProgress(id);
      setActionMessage('Вы записаны на курс.');
      toast.success('Вы записаны на курс');
    } catch (err: unknown) {
      const detail =
        err &&
        typeof err === 'object' &&
        'response' in err &&
        (err as { response?: { data?: { detail?: string } } }).response?.data?.detail;
      const message = typeof detail === 'string' ? detail : 'Не удалось записаться.';
      setActionMessage(message);
      toast.error(message);
    } finally {
      setEnrolling(false);
    }
  };

  if (course === undefined) {
    return <CourseDetailLoading />;
  }

  if (error || !course) {
    return <CourseDetailError message={error} />;
  }

  const coverSrc =
    course.coverImageBucket && course.coverImageKey ? `${baseURL}/api/courses/${course.id}/cover` : null;

  const modules = [...course.modules].sort((a, b) => a.order - b.order);
  const completedSet = new Set(progress?.completedLessonIds ?? []);
  const lockedSet = new Set(progress?.lockedLessonIds ?? []);
  const lessonCount = countLessonsFromDetail(modules);
  const moduleCount = modules.length;

  return (
    <div className={styles.page}>
      <Breadcrumbs
        items={[
          { label: 'Каталог', to: '/courses' },
          { label: course.title },
        ]}
      />

      <article className={styles.heroCard}>
        {coverSrc && (
          <div className={styles.coverWrap}>
            <CourseCover src={coverSrc} />
          </div>
        )}

        <div className={styles.titleRow}>
          <h1 className={styles.title}>{course.title}</h1>
          {localStorage.getItem('accessToken') && (
            <Link to={`/admin/courses/${course.id}/edit`} className={styles.editLink}>
              Редактировать
            </Link>
          )}
        </div>
        <p className={styles.description}>{course.description}</p>

        <div className={styles.meta}>
          <span className={styles.metaItem}>{moduleCount} модулей</span>
          <span className={styles.metaItem}>{lessonCount} уроков</span>
          <span className={`${styles.metaItem} ${styles.price}`}>
            {course.price > 0 ? `${course.price.toFixed(0)} ₽` : 'Бесплатно'}
          </span>
        </div>

        {progress?.isEnrolled && <CourseProgressSection progress={progress} />}

        {!progress?.isEnrolled && (
          <EnrollSection enrolling={enrolling} onEnroll={onEnroll} actionMessage={actionMessage} />
        )}

        {progress?.isEnrolled && actionMessage && (
          <p className={styles.enrolledMessage}>{actionMessage}</p>
        )}
      </article>

      <section className={styles.programSection}>
        <CourseProgram
          courseId={course.id}
          modules={modules}
          completedLessonIds={completedSet}
          lockedLessonIds={lockedSet}
          isEnrolled={Boolean(progress?.isEnrolled)}
        />
      </section>
    </div>
  );
};
