import { NavLink, useNavigate } from 'react-router-dom';
import { useAuth } from '../../hooks/useAuth';
import styles from './AppSidebar.module.scss';

const navLinkClass = ({ isActive }: { isActive: boolean }) =>
  isActive ? `${styles.link} ${styles.linkActive}` : styles.link;

export const AppSidebar = () => {
  const { isLoggedIn, isAdmin, logout } = useAuth();
  const navigate = useNavigate();

  const onLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <aside className={styles.sidebar}>
      <NavLink to="/courses" className={styles.brand}>
        <span className={styles.logoMark} aria-hidden>
          C
        </span>
        <span className={styles.brandText}>
          <span className={styles.brandName}>CTS Academy</span>
          <span className={styles.brandTagline}>Корпоративное обучение</span>
        </span>
      </NavLink>

      <nav className={styles.nav} aria-label="Основное меню">
        <NavLink to="/courses" className={navLinkClass} end>
          Каталог курсов
        </NavLink>
        {isLoggedIn && (
          <NavLink to="/my-learning" className={navLinkClass}>
            Моё обучение
          </NavLink>
        )}
        {isAdmin && (
          <>
            <NavLink to="/admin/analytics" className={navLinkClass}>
              Аналитика
            </NavLink>
            <NavLink to="/admin/courses/new" className={navLinkClass}>
              Создать курс
            </NavLink>
            <NavLink to="/admin/users" className={navLinkClass}>
              Пользователи
            </NavLink>
          </>
        )}
      </nav>

      <div className={styles.footer}>
        {isLoggedIn ? (
          <>
            <span className={styles.userBadge}>Вы в системе</span>
            <button type="button" className={styles.logoutBtn} onClick={onLogout}>
              Выйти
            </button>
          </>
        ) : (
          <>
            <NavLink to="/login" className={styles.authLink}>
              Вход
            </NavLink>
            <NavLink to="/register" className={`${styles.authLink} ${styles.authLinkPrimary}`}>
              Регистрация
            </NavLink>
          </>
        )}
      </div>
    </aside>
  );
};
