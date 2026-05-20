import type { ReactNode } from 'react';
import styles from './AuthScreen.module.scss';

type Props = {
  children: ReactNode;
};

export const AuthScreen = ({ children }: Props) => (
  <div className={styles.screen}>
    <div className={styles.card}>{children}</div>
  </div>
);
