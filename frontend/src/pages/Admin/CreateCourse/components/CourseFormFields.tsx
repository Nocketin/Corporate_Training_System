import { CourseStructureEditor } from '../../courseStructure/CourseStructureEditor';
import type { ModuleDraft } from '../../courseStructure/types';
import styles from './CourseFormFields.module.scss';

type Props = {
  title: string;
  description: string;
  price: string;
  modules: ModuleDraft[];
  onTitleChange: (v: string) => void;
  onDescriptionChange: (v: string) => void;
  onPriceChange: (v: string) => void;
  onModulesChange: (modules: ModuleDraft[]) => void;
  onDeleteResource: (serverId: string) => void;
  onCoverChange: (file: File | null) => void;
};

export const CourseFormFields = ({
  title,
  description,
  price,
  modules,
  onTitleChange,
  onDescriptionChange,
  onPriceChange,
  onModulesChange,
  onDeleteResource,
  onCoverChange,
}: Props) => (
  <>
    <label className={styles.field}>
      <span className={styles.label}>Название (&gt;5 символов)</span>
      <input
        className={styles.input}
        value={title}
        onChange={(e) => onTitleChange(e.target.value)}
        required
        minLength={6}
      />
    </label>
    <label className={styles.field}>
      <span className={styles.label}>Описание</span>
      <textarea
        className={styles.textarea}
        value={description}
        onChange={(e) => onDescriptionChange(e.target.value)}
        required
        rows={4}
      />
    </label>
    <label className={styles.field}>
      <span className={styles.label}>Цена</span>
      <input
        className={styles.input}
        type="number"
        min={0}
        step="0.01"
        value={price}
        onChange={(e) => onPriceChange(e.target.value)}
      />
    </label>
    <label className={styles.field}>
      <span className={styles.label}>Обложка (файл)</span>
      <input
        className={styles.file}
        type="file"
        accept="image/*"
        onChange={(e) => onCoverChange(e.target.files?.[0] ?? null)}
      />
    </label>

    <CourseStructureEditor
      modules={modules}
      onChange={onModulesChange}
      onDeleteResource={onDeleteResource}
    />
  </>
);
