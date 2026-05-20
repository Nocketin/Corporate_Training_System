import { FormEvent, useMemo, useState } from 'react';
import { toast } from 'react-toastify';
import { Link, useNavigate } from 'react-router-dom';
import { createCourse } from '../../../api/courses';
import { buildCourseFormData } from '../courseStructure/buildCourseFormData';
import { createEmptyModule, type ModuleDraft } from '../courseStructure/types';
import { AuthorAuthWarning } from './components/AuthorAuthWarning';
import { CourseFormFields } from './components/CourseFormFields';
import styles from './CreateCoursePage.module.scss';

export const CreateCoursePage = () => {
  const navigate = useNavigate();
  const authorId = useMemo(() => localStorage.getItem('userId') ?? '', []);
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [price, setPrice] = useState('0');
  const [modules, setModules] = useState<ModuleDraft[]>(() => [createEmptyModule(0)]);
  const [deletedResourceIds, setDeletedResourceIds] = useState<string[]>([]);
  const [cover, setCover] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const trackDeletedResource = (serverId: string) => {
    setDeletedResourceIds((prev) => (prev.includes(serverId) ? prev : [...prev, serverId]));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    if (!authorId) {
      setError('Сначала войдите в систему (нужен AuthorId из токена).');
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const form = buildCourseFormData(
        {
          title,
          description,
          authorId,
          price: parseFloat(price) || 0,
          cover,
          modules,
          deletedResourceIds,
        },
        'create',
      );

      const created = await createCourse(form);
      toast.success('Курс создан');
      navigate(`/courses/${created.id}`);
    } catch (err: unknown) {
      const ax = err as { response?: { data?: { detail?: string } } };
      setError(ax.response?.data?.detail ?? 'Ошибка создания курса');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={styles.page}>
      <Link to="/courses" className={styles.back}>
        ← Витрина
      </Link>
      <h1 className={styles.title}>Новый курс</h1>

      {!authorId && <AuthorAuthWarning />}

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
          {loading ? 'Создание…' : 'Создать курс'}
        </button>
      </form>
    </div>
  );
};
