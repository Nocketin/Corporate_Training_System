import { Navigate, Route, Routes } from 'react-router-dom';
import { AppLayout } from './components/AppLayout/AppLayout';
import { AuthLayout } from './components/AuthLayout/AuthLayout';
import { LoginPage } from './pages/Login/LoginPage';
import { RegisterPage } from './pages/Register/RegisterPage';
import { CoursesListPage } from './pages/CoursesList/CoursesListPage';
import { CourseDetailPage } from './pages/CourseDetail/CourseDetailPage';
import { CreateCoursePage } from './pages/Admin/CreateCourse/CreateCoursePage';
import { EditCoursePage } from './pages/Admin/EditCourse/EditCoursePage';
import { LessonPage } from './pages/Lesson/LessonPage';
import { RequireAdmin } from './components/RequireAdmin/RequireAdmin';
import { AnalyticsPage } from './pages/Admin/Analytics/AnalyticsPage';
import { MyLearningPage } from './pages/MyLearning/MyLearningPage';
import { AdminUsersPage } from './pages/Admin/Users/AdminUsersPage';
import { ForgotPasswordPage } from './pages/ForgotPassword/ForgotPasswordPage';
import { ResetPasswordPage } from './pages/ResetPassword/ResetPasswordPage';

export const App = () => (
  <Routes>
    <Route path="/" element={<Navigate to="/courses" replace />} />

    <Route element={<AuthLayout />}>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/forgot-password" element={<ForgotPasswordPage />} />
      <Route path="/reset-password" element={<ResetPasswordPage />} />
    </Route>

    <Route element={<AppLayout />}>
      <Route path="/courses" element={<CoursesListPage />} />
      <Route path="/courses/:id" element={<CourseDetailPage />} />
      <Route path="/courses/:courseId/lessons/:lessonId" element={<LessonPage />} />
      <Route path="/my-learning" element={<MyLearningPage />} />
      <Route element={<RequireAdmin />}>
        <Route path="/admin/analytics" element={<AnalyticsPage />} />
        <Route path="/admin/users" element={<AdminUsersPage />} />
        <Route path="/admin/courses/new" element={<CreateCoursePage />} />
        <Route path="/admin/courses/:id/edit" element={<EditCoursePage />} />
      </Route>
    </Route>

    <Route path="*" element={<Navigate to="/courses" replace />} />
  </Routes>
);
