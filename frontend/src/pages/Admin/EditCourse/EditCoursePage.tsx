import { FormEvent, useEffect, useState } from 'react';
import { toast } from 'react-toastify';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { fetchCourse, updateCourse } from '../../../api/courses';
import { buildCourseFormData } from '../courseStructure/buildCourseFormData';
import { courseDetailToDraft } from '../courseStructure/courseDetailToDraft';
import type { ModuleDraft } from '../courseStructure/types';
import { CourseFormFields } from '../CreateCourse/components/CourseFormFields';
import styles from '../CreateCourse/CreateCoursePage.module.scss';

export const EditCoursePage = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('0');
  const [modules, setModules] = useState<ModuleDraft[]>([]);
  const [deletedResourceIds, setDeletedResourceIds] = useState<string[]>([]);
  const [cover, setCover] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [initialLoading, setInitialLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    let cancelled = false;
    (async () => {
      try {
        const course = await fetchCourse(id);
        if (cancelled) return;
        setTitle(course.title);
        setDescription(course.description);
        setPrice(String(course.price));
        setModules(courseDetailToDraft(course));
      } catch {
        if (!cancelled) setError('Не удалось загрузить курс');
      } finally {
        if (!cancelled) setInitialLoading(false);
      }
    })();
    return () => {
      cancelled = true;
    };
  }, [id]);

  const trackDeletedResource = (serverId: string) => {
    setDeletedResourceIds((prev) => (prev.includes(serverId) ? prev : [...prev, serverId]));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!id) return;

    setLoading(true);
    setError(null);

    try {
      const form = buildCourseFormData(
        {
          title,
          description,
          price: parseFloat(price) || 0,
          cover,
          modules,
          deletedResourceIds,
        },
        'update',
      );

      const updated = await updateCourse(id, form);
      toast.success('Курс сохранён');
      navigate(`/courses/${updated.id}`);
    } catch (err: unknown) {
      const ax = err as { response?: { data?: { detail?: string } } };
      setError(ax.response?.data?.detail ?? 'Ошибка сохранения курса');
    } finally {
      setLoading(false);
    }
  };

  if (initialLoading) {
    return <p className={styles.error}>Загрузка…</p>;
  }

  return (
    <div className={styles.page}>
      <Link to={id ? `/courses/${id}` : '/courses'} className={styles.back}>
        ← К курсу
      </Link>
      <h1 className={styles.title}>Редактирование курса</h1>

      <form className={styles.form} onSubmit={handleSubmit}>
        <CourseFormFields
          title={title}
          description={description}
          price={price}
          modules={modules}
          onTitleChange={setTitle}
          onDescriptionChange={setDescription}
          onPriceChange={setPrice}
          onModulesChange={setModules}
          onDeleteResource={trackDeletedResource}
          onCoverChange={setCover}
        />

        {error && <p className={styles.error}>{error}</p>}

        <button type="submit" className={styles.submit} disabled={loading}>
          {loading ? 'Сохранение…' : 'Сохранить изменения'}
        </button>
      </form>
    </div>
  );
};
