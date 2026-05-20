import styles from './CourseCover.module.scss';

type Props = {
  src: string;
  alt?: string;
};

export const CourseCover = ({ src, alt = '' }: Props) => (
  <img className={styles.cover} src={src} alt={alt} />
);
