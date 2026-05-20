import styles from './AuthErrorBanner.module.scss';

type Props = {
  message: string;
};

export const AuthErrorBanner = ({ message }: Props) => (
  <div className={styles.banner} role="alert">
    {message}
  </div>
);
