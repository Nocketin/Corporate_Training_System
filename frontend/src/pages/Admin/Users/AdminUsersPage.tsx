import { useEffect, useState } from 'react';
import { fetchUsers, updateUserRole, type UserListItem } from '../../../api/users';
import { PageHeader } from '../../../components/PageHeader/PageHeader';
import { toast } from 'react-toastify';
import styles from './AdminUsersPage.module.scss';

export const AdminUsersPage = () => {
  const [users, setUsers] = useState<UserListItem[] | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [savingId, setSavingId] = useState<string | null>(null);

  const load = async () => {
    try {
      const data = await fetchUsers();
      setUsers(data);
      setError(null);
    } catch {
      setError('Не удалось загрузить пользователей');
    }
  };

  useEffect(() => {
    void load();
  }, []);

  const onRoleChange = async (user: UserListItem, role: string) => {
    if (user.role === role) return;
    setSavingId(user.id);
    try {
      const updated = await updateUserRole(user.id, role);
      setUsers((prev) => prev?.map((u) => (u.id === updated.id ? updated : u)) ?? null);
      toast.success('Роль обновлена');
    } catch {
      toast.error('Не удалось обновить роль');
    } finally {
      setSavingId(null);
    }
  };

  return (
    <div className={styles.page}>
      <PageHeader title="Пользователи" subtitle="Управление ролями" />

      {error && <p className={styles.error}>{error}</p>}
      {users === null && !error && <p>Загрузка…</p>}

      {users && (
        <table className={styles.table}>
          <thead>
            <tr>
              <th>Email</th>
              <th>Имя</th>
              <th>Роль</th>
              <th>Регистрация</th>
            </tr>
          </thead>
          <tbody>
            {users.map((u) => (
              <tr key={u.id}>
                <td>{u.email}</td>
                <td>{u.username}</td>
                <td>
                  <select
                    className={styles.select}
                    value={u.role}
                    disabled={savingId === u.id}
                    onChange={(e) => void onRoleChange(u, e.target.value)}
                  >
                    <option value="User">User</option>
                    <option value="Admin">Admin</option>
                  </select>
                </td>
                <td>{new Date(u.createdAt).toLocaleDateString('ru-RU')}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};
