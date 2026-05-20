import {
  createFileResource,
  createLinkResource,
  type LessonDraft,
  type ResourceDraft,
} from './types';
import styles from './CourseStructureEditor.module.scss';

type Props = {
  lesson: LessonDraft;
  onChange: (lesson: LessonDraft) => void;
  onDeleteResource: (serverId: string) => void;
};

export const LessonResourcesEditor = ({ lesson, onChange, onDeleteResource }: Props) => {
  const updateResources = (resources: ResourceDraft[]) => {
    onChange({ ...lesson, resources });
  };

  const updateResource = (clientId: string, patch: Partial<ResourceDraft>) => {
    updateResources(
      lesson.resources.map((r) => (r.clientId === clientId ? ({ ...r, ...patch } as ResourceDraft) : r)),
    );
  };

  const removeResource = (resource: ResourceDraft) => {
    if (resource.serverId) {
      onDeleteResource(resource.serverId);
    }
    updateResources(lesson.resources.filter((r) => r.clientId !== resource.clientId));
  };

  return (
    <div className={styles.resourcesBlock}>
      <p className={styles.resourcesTitle}>Материалы урока</p>

      {lesson.resources.map((resource) => (
        <div key={resource.clientId} className={styles.resourceRow}>
          <div className={styles.resourceRowHeader}>
            <span className={styles.resourceKind}>
              {resource.kind === 'link' ? 'Ссылка' : 'Файл'}
            </span>
            <button type="button" className={styles.btnDanger} onClick={() => removeResource(resource)}>
              Удалить
            </button>
          </div>

          <label className={styles.field}>
            <span className={styles.label}>Название</span>
            <input
              className={styles.input}
              value={resource.title}
              onChange={(e) => updateResource(resource.clientId, { title: e.target.value })}
              placeholder={resource.kind === 'link' ? 'Видео на YouTube' : 'Презентация'}
            />
          </label>

          {resource.kind === 'link' ? (
            <label className={styles.field}>
              <span className={styles.label}>URL</span>
              <input
                className={styles.input}
                type="url"
                value={resource.url}
                onChange={(e) => updateResource(resource.clientId, { url: e.target.value })}
                placeholder="https://www.youtube.com/watch?v=..."
              />
            </label>
          ) : (
            <label className={styles.field}>
              <span className={styles.label}>Файл (PDF, DOC, DOCX)</span>
              <input
                className={styles.input}
                type="file"
                accept=".pdf,.doc,.docx,application/pdf,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                onChange={(e) => {
                  const file = e.target.files?.[0] ?? null;
                  updateResource(resource.clientId, { file, existingFileName: file?.name });
                }}
              />
              <p className={styles.fileHint}>
                {resource.file?.name ??
                  resource.existingFileName ??
                  'Выберите файл или оставьте текущий при редактировании'}
              </p>
            </label>
          )}
        </div>
      ))}

      <div className={styles.rowActions}>
        <button
          type="button"
          className={styles.btnSecondary}
          onClick={() =>
            updateResources([...lesson.resources, createLinkResource(lesson.resources.length)])
          }
        >
          + Ссылка
        </button>
        <button
          type="button"
          className={styles.btnSecondary}
          onClick={() =>
            updateResources([...lesson.resources, createFileResource(lesson.resources.length)])
          }
        >
          + Файл
        </button>
      </div>
    </div>
  );
};
