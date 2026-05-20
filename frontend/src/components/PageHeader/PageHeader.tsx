import type { ReactNode } from 'react';
import styles from './PageHeader.module.scss';

type Props = {
  title: string;
  subtitle?: string;
  actions?: ReactNode;
};

export const PageHeader = ({ title, subtitle, actions }: Props) => (
  <header className={styles.header}>
    <div className={styles.text}>
      <h1 className={styles.title}>{title}</h1>
      {subtitle && <p className={styles.subtitle}>{subtitle}</p>}
    </div>
    {actions && <div className={styles.actions}>{actions}</div>}
  </header>
);
