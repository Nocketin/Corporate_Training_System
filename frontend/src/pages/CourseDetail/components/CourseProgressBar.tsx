import type { CSSProperties } from 'react';
import styles from './CourseProgressBar.module.scss';

type Props = {
  percent: number;
  isCompleted: boolean;
};

export const CourseProgressBar = ({ percent, isCompleted }: Props) => {
  const clamped = Math.min(100, Math.max(0, percent));
  const cssVars = { '--progress': `${clamped}%` } as CSSProperties;

  return (
    <div className={styles.track}>
      <div
        className={`${styles.fill} ${isCompleted ? styles.fillCompleted : styles.fillActive}`}
        style={cssVars}
      />
    </div>
  );
};
