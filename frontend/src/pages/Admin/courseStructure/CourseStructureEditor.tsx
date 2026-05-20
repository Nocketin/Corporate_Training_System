import { LessonResourcesEditor } from './LessonResourcesEditor';
import {
  createEmptyLesson,
  createEmptyModule,
  type LessonDraft,
  type ModuleDraft,
} from './types';
import styles from './CourseStructureEditor.module.scss';

type Props = {
  modules: ModuleDraft[];
  onChange: (modules: ModuleDraft[]) => void;
  onDeleteResource: (serverId: string) => void;
};

export const CourseStructureEditor = ({
  modules,
  onChange,
  onDeleteResource,
}: Props) => {
  const updateModules = (next: ModuleDraft[]) => onChange(next);

  const updateModule = (clientId: string, patch: Partial<ModuleDraft>) => {
    updateModules(modules.map((m) => (m.clientId === clientId ? { ...m, ...patch } : m)));
  };

  const removeModule = (clientId: string) => {
    const module = modules.find((m) => m.clientId === clientId);
    module?.lessons.forEach((lesson) => {
      lesson.resources.forEach((r) => {
        if (r.serverId) onDeleteResource(r.serverId);
      });
    });
    updateModules(modules.filter((m) => m.clientId !== clientId));
  };

  const updateLesson = (moduleClientId: string, lesson: LessonDraft) => {
    updateModules(
      modules.map((m) =>
        m.clientId === moduleClientId
          ? { ...m, lessons: m.lessons.map((l) => (l.clientId === lesson.clientId ? lesson : l)) }
          : m,
      ),
    );
  };

  const addLesson = (moduleClientId: string) => {
    updateModules(
      modules.map((m) =>
        m.clientId === moduleClientId
          ? { ...m, lessons: [...m.lessons, createEmptyLesson(m.lessons.length)] }
          : m,
      ),
    );
  };

  const removeLesson = (moduleClientId: string, lessonClientId: string) => {
    const module = modules.find((m) => m.clientId === moduleClientId);
    const lesson = module?.lessons.find((l) => l.clientId === lessonClientId);
    lesson?.resources.forEach((r) => {
      if (r.serverId) onDeleteResource(r.serverId);
    });
    updateModules(
      modules.map((m) =>
        m.clientId === moduleClientId
          ? { ...m, lessons: m.lessons.filter((l) => l.clientId !== lessonClientId) }
          : m,
      ),
    );
  };

  return (
    <section className={styles.section}>
      <h2 className={styles.sectionTitle}>Программа курса</h2>

      {modules.map((module, moduleIndex) => (
        <article key={module.clientId} className={styles.moduleCard}>
          <header className={styles.moduleHeader}>
            <p className={styles.moduleLabel}>Модуль {moduleIndex + 1}</p>
            {modules.length > 1 && (
              <button
                type="button"
                className={styles.btnDanger}
                onClick={() => removeModule(module.clientId)}
              >
                Удалить модуль
              </button>
            )}
          </header>

          <label className={styles.field}>
            <span className={styles.label}>Название модуля</span>
            <input
              className={styles.input}
              value={module.title}
              onChange={(e) => updateModule(module.clientId, { title: e.target.value })}
              required
            />
          </label>

          {module.lessons.map((lesson, lessonIndex) => (
            <div key={lesson.clientId} className={styles.lessonCard}>
              <header className={styles.moduleHeader}>
                <p className={styles.moduleLabel}>Урок {lessonIndex + 1}</p>
                {module.lessons.length > 1 && (
                  <button
                    type="button"
                    className={styles.btnDanger}
                    onClick={() => removeLesson(module.clientId, lesson.clientId)}
                  >
                    Удалить урок
                  </button>
                )}
              </header>

              <label className={styles.field}>
                <span className={styles.label}>Название урока</span>
                <input
                  className={styles.input}
                  value={lesson.title}
                  onChange={(e) =>
                    updateLesson(module.clientId, { ...lesson, title: e.target.value })
                  }
                  required
                />
              </label>

              <label className={styles.field}>
                <span className={styles.label}>Длительность (мин)</span>
                <input
                  className={styles.input}
                  type="number"
                  min={0}
                  value={lesson.durationMinutes}
                  onChange={(e) =>
                    updateLesson(module.clientId, {
                      ...lesson,
                      durationMinutes: Number(e.target.value) || 0,
                    })
                  }
                />
              </label>

              <label className={styles.field}>
                <span className={styles.label}>Текст урока</span>
                <textarea
                  className={styles.textarea}
                  value={lesson.textContent}
                  onChange={(e) =>
                    updateLesson(module.clientId, { ...lesson, textContent: e.target.value })
                  }
                  rows={3}
                />
              </label>

              <LessonResourcesEditor
                lesson={lesson}
                onChange={(next) => updateLesson(module.clientId, next)}
                onDeleteResource={onDeleteResource}
              />
            </div>
          ))}

          <button type="button" className={styles.btnSecondary} onClick={() => addLesson(module.clientId)}>
            + Урок
          </button>
        </article>
      ))}

      <button
        type="button"
        className={styles.btnGhost}
        onClick={() => updateModules([...modules, createEmptyModule(modules.length)])}
      >
        + Модуль
      </button>
    </section>
  );
};
