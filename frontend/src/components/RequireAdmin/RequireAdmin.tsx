import { Navigate, Outlet } from 'react-router-dom';
import { isAdmin, isAuthenticated } from '../../lib/auth';

export const RequireAdmin = () => {
  if (!isAuthenticated()) {
    return <Navigate to="/login" replace />;
  }

  if (!isAdmin()) {
    return <Navigate to="/courses" replace />;
  }

  return <Outlet />;
};
