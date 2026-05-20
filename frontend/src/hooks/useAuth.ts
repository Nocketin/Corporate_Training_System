import { useCallback, useEffect, useState } from 'react';
import { clearSession, isAdmin, isAuthenticated } from '../lib/auth';

export const useAuth = () => {
  const [loggedIn, setLoggedIn] = useState(isAuthenticated);
  const [admin, setAdmin] = useState(isAdmin);

  const sync = useCallback(() => {
    setLoggedIn(isAuthenticated());
    setAdmin(isAdmin());
  }, []);

  useEffect(() => {
    sync();
    const onStorage = (e: StorageEvent) => {
      if (
        e.key === null ||
        e.key === 'accessToken' ||
        e.key === 'refreshToken' ||
        e.key === 'userId' ||
        e.key === 'userRole'
      ) {
        sync();
      }
    };
    window.addEventListener('storage', onStorage);
    return () => window.removeEventListener('storage', onStorage);
  }, [sync]);

  const logout = useCallback(() => {
    clearSession();
    setLoggedIn(false);
    setAdmin(false);
  }, []);

  return { isLoggedIn: loggedIn, isAdmin: admin, logout, refreshAuth: sync };
};
