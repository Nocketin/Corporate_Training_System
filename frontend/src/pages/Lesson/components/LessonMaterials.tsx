import { baseURL } from '../../../api/axiosInstance';
import type { LessonDto, LessonResourceDto } from '../../../api/courses';
import { getLessonResourceFileUrl } from '../../../api/courses';
import { getYouTubeEmbedUrl, isYouTubeUrl } from '../../../lib/youtube';
import styles from './LessonMaterials.module.scss';

type Props = {
  lesson: LessonDto;
};

function resolveFileUrl(lessonId: string, resource: LessonResourceDto): string {
  if (resource.downloadUrl) {
    return resource.downloadUrl.startsWith('http')
      ? resource.downloadUrl
      : `${baseURL}${resource.downloadUrl}`;
  }
  return `${baseURL}${getLessonResourceFileUrl(lessonId, resource.id)}`;
}

function isPdf(resource: LessonResourceDto): boolean {
  const name = resource.fileName?.toLowerCase() ?? '';
  const type = resource.contentType?.toLowerCase() ?? '';
  return name.endsWith('.pdf') || type.includes('pdf');
}

function isWord(resource: LessonResourceDto): boolean {
  const name = resource.fileName?.toLowerCase() ?? '';
  const type = resource.contentType?.toLowerCase() ?? '';
  return (
    name.endsWith('.doc') ||
    name.endsWith('.docx') ||
    type.includes('msword') ||
    type.includes('wordprocessingml')
  );
}

export const LessonMaterials = ({ lesson }: Props) => {
  const resources = [...lesson.resources].sort((a, b) => a.order - b.order);

  if (resources.length === 0 && lesson.contentUrl) {
    const embed = getYouTubeEmbedUrl(lesson.contentUrl);
    return (
      <section className={styles.section}>
        <h2 className={styles.title}>Материалы</h2>
        <div className={styles.list}>
          {embed ? (
            <article className={styles.card}>
              <div className={styles.videoWrap}>
                <iframe
                  className={styles.videoFrame}
                  src={embed}
                  title="Видео"
                  allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                  allowFullScreen
                />
              </div>
            </article>
          ) : (
            <article className={styles.card}>
              <a className={styles.linkBtn} href={lesson.contentUrl} target="_blank" rel="noreferrer">
                Открыть дополнительный материал ↗
              </a>
            </article>
          )}
        </div>
      </section>
    );
  }

  if (resources.length === 0) {
    return null;
  }

  return (
    <section className={styles.section}>
      <h2 className={styles.title}>Материалы</h2>
      <div className={styles.list}>
        {resources.map((resource) => {
          if (resource.kind.toLowerCase() === 'link' && resource.url) {
            const embed = isYouTubeUrl(resource.url) ? getYouTubeEmbedUrl(resource.url) : null;
            return (
              <article key={resource.id} className={styles.card}>
                <header className={styles.cardHeader}>
                  <h3 className={styles.cardTitle}>{resource.title}</h3>
                  <span className={styles.badge}>{embed ? 'YouTube' : 'Ссылка'}</span>
                </header>
                {embed ? (
                  <div className={styles.videoWrap}>
                    <iframe
                      className={styles.videoFrame}
                      src={embed}
                      title={resource.title}
                      allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"
                      allowFullScreen
                    />
                  </div>
                ) : (
                  <a className={styles.linkBtn} href={resource.url} target="_blank" rel="noreferrer">
                    Открыть ↗
                  </a>
                )}
              </article>
            );
          }

          if (resource.kind.toLowerCase() === 'file') {
            const fileUrl = resolveFileUrl(lesson.id, resource);
            const pdf = isPdf(resource);
            const word = isWord(resource);

            return (
              <article key={resource.id} className={styles.card}>
                <header className={styles.cardHeader}>
                  <h3 className={styles.cardTitle}>{resource.title}</h3>
                  <span className={styles.badge}>{pdf ? 'PDF' : word ? 'Word' : 'Файл'}</span>
                </header>
                {resource.fileName && (
                  <p className={styles.fileMeta}>{resource.fileName}</p>
                )}
                {pdf ? (
                  <iframe className={styles.pdfFrame} src={fileUrl} title={resource.title} />
                ) : null}
                <a className={styles.linkBtn} href={fileUrl} target="_blank" rel="noreferrer">
                  {pdf ? 'Скачать PDF' : word ? 'Скачать документ' : 'Скачать файл'} ↗
                </a>
              </article>
            );
          }

          return null;
        })}
      </div>
    </section>
  );
};
