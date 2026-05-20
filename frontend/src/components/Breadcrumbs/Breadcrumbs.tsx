import { Link } from 'react-router-dom';
import styles from './Breadcrumbs.module.scss';

export type BreadcrumbItem = {
  label: string;
  to?: string;
};

type Props = {
  items: BreadcrumbItem[];
};

export const Breadcrumbs = ({ items }: Props) => (
  <nav className={styles.nav} aria-label="Хлебные крошки">
    <ol className={styles.list}>
      {items.map((item, index) => {
        const isLast = index === items.length - 1;
        return (
          <li key={`${item.label}-${index}`} className={styles.item}>
            {item.to && !isLast ? (
              <Link to={item.to}>{item.label}</Link>
            ) : (
              <span className={isLast ? styles.current : undefined} aria-current={isLast ? 'page' : undefined}>
                {item.label}
              </span>
            )}
            {!isLast && <span className={styles.sep} aria-hidden>/</span>}
          </li>
        );
      })}
    </ol>
  </nav>
);
