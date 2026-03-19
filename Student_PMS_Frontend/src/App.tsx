import { BrowserRouter as Router, Navigate, Route, Routes } from "react-router";
import Videos from "./pages/UiElements/Videos";
import Images from "./pages/UiElements/Images";
import Alerts from "./pages/UiElements/Alerts";
import Badges from "./pages/UiElements/Badges";
import Avatars from "./pages/UiElements/Avatars";
import Buttons from "./pages/UiElements/Buttons";
import LineChart from "./pages/Charts/LineChart";
import BarChart from "./pages/Charts/BarChart";
import Calendar from "./pages/Calendar";
import BasicTables from "./pages/Tables/BasicTables";
import FormElements from "./pages/Forms/FormElements";
import Blank from "./pages/Blank";
import AppLayout from "./layout/AppLayout";
import { ScrollToTop } from "./components/common/ScrollToTop";
import Home from "./pages/Dashboard/Home";
import {
  AuthProvider,
  GuestRoute,
  ProtectedRoute,
} from "./AuthServices/components";
import { SignInPage, SignUpPage } from "./AuthServices/pages";
import {
  ExceptionBoundary,
  GlobalExceptionProvider,
} from "./Exception/Components";
import { ExceptionStatusGuard } from "./Exception/Guards";
import { ExceptionStatusPage } from "./Exception/Pages";
import {
  MyAccountPage,
  RoleDetailPage,
  RoleFormPage,
  RolesPage,
  UserDetailPage,
  UserFormPage,
  UsersPage,
} from "./UserServices/pages";
import {
  ProjectDetailPage,
  ProjectFormPage,
  ProjectsPage,
} from "./ProjectServices/pages";

export default function App() {
  return (
    <>
      <Router>
        <ScrollToTop />
        <GlobalExceptionProvider>
          <AuthProvider>
            <ExceptionBoundary>
              <Routes>
                <Route element={<GuestRoute />}>
                  <Route path="/signin" element={<SignInPage />} />
                  <Route path="/signup" element={<SignUpPage />} />
                </Route>

                {/* Dashboard Layout */}
                <Route element={<ProtectedRoute />}>
                  <Route element={<AppLayout />}>
                    <Route index path="/" element={<Home />} />

                    {/* Others Page */}
                    <Route path="/profile" element={<MyAccountPage />} />
                    <Route path="/account" element={<MyAccountPage />} />
                    <Route path="/calendar" element={<Calendar />} />
                    <Route path="/blank" element={<Blank />} />

                    {/* Forms */}
                    <Route path="/form-elements" element={<FormElements />} />

                    {/* Tables */}
                    <Route path="/basic-tables" element={<BasicTables />} />

                    {/* Ui Elements */}
                    <Route path="/alerts" element={<Alerts />} />
                    <Route path="/avatars" element={<Avatars />} />
                    <Route path="/badge" element={<Badges />} />
                    <Route path="/buttons" element={<Buttons />} />
                    <Route path="/images" element={<Images />} />
                    <Route path="/videos" element={<Videos />} />

                    {/* Charts */}
                    <Route path="/line-chart" element={<LineChart />} />
                    <Route path="/bar-chart" element={<BarChart />} />

                    {/* Project Services */}
                    <Route path="/projects" element={<ProjectsPage />} />
                    <Route path="/projects/:projectId" element={<ProjectDetailPage />} />
                    <Route element={<ProtectedRoute allowedRoles={["Admin", "Faculty"]} />}>
                      <Route path="/projects/new" element={<ProjectFormPage mode="create" />} />
                      <Route
                        path="/projects/:projectId/edit"
                        element={<ProjectFormPage mode="edit" />}
                      />
                    </Route>

                    {/* User Services */}
                    <Route element={<ProtectedRoute allowedRoles={["Admin"]} />}>
                      <Route path="/users" element={<UsersPage />} />
                      <Route path="/users/new" element={<UserFormPage mode="create" />} />
                      <Route path="/users/:userId" element={<UserDetailPage />} />
                      <Route
                        path="/users/:userId/edit"
                        element={<UserFormPage mode="edit" />}
                      />

                      <Route path="/roles" element={<RolesPage />} />
                      <Route path="/roles/new" element={<RoleFormPage mode="create" />} />
                      <Route path="/roles/:roleId" element={<RoleDetailPage />} />
                      <Route
                        path="/roles/:roleId/edit"
                        element={<RoleFormPage mode="edit" />}
                      />
                    </Route>
                  </Route>
                </Route>

                {/* Exception Pages */}
                <Route path="/error-400" element={<ExceptionStatusPage statusCode={400} />} />
                <Route path="/error-401" element={<ExceptionStatusPage statusCode={401} />} />
                <Route path="/error-404" element={<ExceptionStatusPage statusCode={404} />} />
                <Route path="/error-409" element={<ExceptionStatusPage statusCode={409} />} />
                <Route path="/error-500" element={<ExceptionStatusPage statusCode={500} />} />
                <Route path="/error-503" element={<ExceptionStatusPage statusCode={503} />} />
                <Route element={<ExceptionStatusGuard />}>
                  <Route path="/exceptions/:statusCode" element={<ExceptionStatusPage />} />
                </Route>

                {/* Fallback Route */}
                <Route path="*" element={<Navigate to="/error-404" replace />} />
              </Routes>
            </ExceptionBoundary>
          </AuthProvider>
        </GlobalExceptionProvider>
      </Router>
    </>
  );
}
