import styles from './LessonTextContent.module.scss';

type Props = {
  text: string;
};

export const LessonTextContent = ({ text }: Props) => (
  <div className={styles.box}>{text}</div>
);
