import { Link } from 'react-router-dom';
import { baseURL } from '../../../api/axiosInstance';
import type { CourseListItem } from '../../../api/courses';
import { getCourseAccent } from '../../../lib/courseAccent';
import styles from './CourseCard.module.scss';

type Props = {
  course: CourseListItem;
};

export const CourseCard = ({ course: c }: Props) => {
  const coverSrc = c.coverImageUrl
    ? c.coverImageUrl.startsWith('http')
      ? c.coverImageUrl
      : `${baseURL}${c.coverImageUrl}`
    : null;

  const accent = getCourseAccent(c.id);
  const description =
    c.description.length > 110 ? `${c.description.slice(0, 110)}…` : c.description;
  const isFree = c.price <= 0;

  return (
    <Link className={styles.card} to={`/courses/${c.id}`}>
      {coverSrc ? (
        <img className={styles.cover} src={coverSrc} alt="" />
      ) : (
        <div className={`${styles.placeholder} ${styles[`accent_${accent}`]}`} aria-hidden>
          <span className={styles.placeholderIcon}>📚</span>
        </div>
      )}
      <div className={styles.body}>
        <div className={styles.meta}>
          <span className={styles.badge}>Курс</span>
          {isFree ? (
            <span className={styles.badgeFree}>Бесплатно</span>
          ) : (
            <span className={styles.price}>{c.price.toFixed(0)} ₽</span>
          )}
        </div>
        <h2 className={styles.cardTitle}>{c.title}</h2>
        <p className={styles.desc}>{description}</p>
        <span className={styles.cta}>Подробнее →</span>
      </div>
    </Link>
  );
};
