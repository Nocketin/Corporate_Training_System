import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { fetchCourse, type CourseDetail, type LessonDto } from '../../api/courses';
import { completeLesson, fetchCourseProgress } from '../../api/learning';
import { Breadcrumbs } from '../../components/Breadcrumbs/Breadcrumbs';
import { LessonCompleteButton } from './components/LessonCompleteButton';
import { LessonLoading } from './components/LessonLoading';
import { LessonNotFound } from './components/LessonNotFound';
import { LessonStatusMessages } from './components/LessonStatusMessages';
import { LessonMaterials } from './components/LessonMaterials';
import { LessonTextContent } from './components/LessonTextContent';
import { toast } from 'react-toastify';
import styles from './LessonPage.module.scss';

function findLesson(course: CourseDetail, lessonId: string): LessonDto | null {
  for (const m of course.modules) {
    const lesson = m.lessons.find((l) => l.id === lessonId);
    if (lesson) return lesson;
  }
  return null;
}

export const LessonPage = () => {
  const { courseId, lessonId } = useParams<{ courseId: string; lessonId: string }>();
  const navigate = useNavigate();
  const [course, setCourse] = useState<CourseDetail | null | undefined>(undefined);
  const [locked, setLocked] = useState(false);
  const [completed, setCompleted] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!courseId || !lessonId) return;
    if (!localStorage.getItem('accessToken')) {
      navigate('/login');
      return;
    }

    let cancelled = false;
    (async () => {
      try {
        const [courseData, progress] = await Promise.all([
          fetchCourse(courseId),
          fetchCourseProgress(courseId),
        ]);
        if (cancelled) return;
        setCourse(courseData);
        if (!progress.isEnrolled) {
          setMessage('Сначала запишитесь на курс.');
          return;
        }
        setCompleted(progress.completedLessonIds.includes(lessonId));
        setLocked(progress.lockedLessonIds.includes(lessonId));
      } catch {
        if (!cancelled) setCourse(null);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [courseId, lessonId, navigate]);

  const onComplete = async () => {
    if (!lessonId || !courseId || locked || completed) return;
    setSubmitting(true);
    setMessage(null);
    try {
      const result = await completeLesson(lessonId);
      setCompleted(true);
      if (result.courseCompleted) {
        const msg = 'Курс завершён! Поздравляем.';
        setMessage(msg);
        toast.success(msg);
      } else {
        const msg = 'Урок отмечен как пройденный.';
        setMessage(msg);
        toast.success(msg);
      }
    } catch (err: unknown) {
      const detail =
        err &&
        typeof err === 'object' &&
        'response' in err &&
        (err as { response?: { data?: { detail?: string } } }).response?.data?.detail;
      const message = typeof detail === 'string' ? detail : 'Не удалось завершить урок.';
      setMessage(message);
      toast.error(message);
    } finally {
      setSubmitting(false);
    }
  };

  if (course === undefined) {
    return <LessonLoading />;
  }

  if (!course || !lessonId || !courseId) {
    return <LessonNotFound courseId={courseId ?? ''} message="Урок не найден" />;
  }

  const lesson = findLesson(course, lessonId);
  if (!lesson) {
    return <LessonNotFound courseId={courseId} message="Урок не найден в программе курса" />;
  }

  return (
    <div className={styles.page}>
      <Breadcrumbs
        items={[
          { label: 'Каталог', to: '/courses' },
          { label: course.title, to: `/courses/${courseId}` },
          { label: lesson.title },
        ]}
      />

      <article className={styles.card}>
        <header className={styles.header}>
          <h1 className={styles.title}>{lesson.title}</h1>
          {lesson.durationMinutes > 0 && (
            <span className={styles.duration}>{lesson.durationMinutes} мин</span>
          )}
        </header>

        {lesson.textContent && <LessonTextContent text={lesson.textContent} />}

        <LessonMaterials lesson={lesson} />

        <LessonStatusMessages locked={locked} completed={completed} message={message} />

        {!locked && !completed && (
          <LessonCompleteButton submitting={submitting} onClick={onComplete} />
        )}
      </article>
    </div>
  );
};
