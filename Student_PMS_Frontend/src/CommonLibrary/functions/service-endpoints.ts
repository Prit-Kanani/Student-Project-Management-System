type Identifier = string | number;

export const apiEndpoints = {
  auth: {
    login: "/api/Auth/login",
    register: "/api/Auth/register",
  },
  userService: {
    user: {
      page: "/api/UserService/User/Page",
      primaryKey: (userId: Identifier) => `/api/UserService/User/Pk/${userId}`,
      view: (userId: Identifier) => `/api/UserService/User/View/${userId}`,
      insert: "/api/UserService/User/Insert",
      update: "/api/UserService/User/Update",
      deactivate: (userId: Identifier) =>
        `/api/UserService/User/Deactivate/${userId}`,
      createdAndModifiedBy: "/api/UserService/User/CreatedAndModifiedBy",
      resolveAuditUsers: "/api/UserService/User/ResolveAuditUsers",
    },
    role: {
      page: "/api/UserService/Role/Page",
      view: (roleId: Identifier) => `/api/UserService/Role/View/${roleId}`,
      primaryKey: (roleId: Identifier) => `/api/UserService/Role/Pk/${roleId}`,
      dropdown: "/api/UserService/Role/Dropdown",
      insert: "/api/UserService/Role/Insert",
      update: "/api/UserService/Role/Update",
      deactivate: (roleId: Identifier) =>
        `/api/UserService/Role/Deactivate/${roleId}`,
    },
    userProfile: {
      page: "/api/UserService/UserProfile/Page",
      view: (userId: Identifier) => `/api/UserService/UserProfile/View/${userId}`,
      primaryKey: (userId: Identifier) =>
        `/api/UserService/UserProfile/Pk/${userId}`,
      insert: "/api/UserService/UserProfile/Insert",
      update: "/api/UserService/UserProfile/Update",
    },
  },
  projectService: {
    project: {
      page: "/api/ProjectService/Project/Page",
      view: (projectId: Identifier) => `/api/ProjectService/Project/View/${projectId}`,
      primaryKey: (projectId: Identifier) =>
        `/api/ProjectService/Project/PK/${projectId}`,
      exists: (projectId: Identifier) => `/api/ProjectService/Project/Exists/${projectId}`,
      create: "/api/ProjectService/Project/Create",
      update: "/api/ProjectService/Project/Update",
      delete: (projectId: Identifier) => `/api/ProjectService/Project/Delete/${projectId}`,
    },
  },
  projectGroupService: {
    projectGroup: {
      page: "/api/ProjectGroupService/ProjectGroup/Page",
      primaryKey: (projectGroupId: Identifier) =>
        `/api/ProjectGroupService/ProjectGroup/PK/${projectGroupId}`,
      view: (projectGroupId: Identifier) =>
        `/api/ProjectGroupService/ProjectGroup/View/${projectGroupId}`,
      create: "/api/ProjectGroupService/ProjectGroup/Create",
      update: "/api/ProjectGroupService/ProjectGroup/Update",
      deactivate: (projectGroupId: Identifier) =>
        `/api/ProjectGroupService/ProjectGroup/Deactivate/${projectGroupId}`,
    },
    projectGroupByProject: {
      page: "/api/ProjectGroupService/ProjectGroupByProject/Page",
      view: (projectGroupByProjectId: Identifier) =>
        `/api/ProjectGroupService/ProjectGroupByProject/View/${projectGroupByProjectId}`,
      primaryKey: (projectGroupByProjectId: Identifier) =>
        `/api/ProjectGroupService/ProjectGroupByProject/PK/${projectGroupByProjectId}`,
      create: "/api/ProjectGroupService/ProjectGroupByProject/Create",
      update: "/api/ProjectGroupService/ProjectGroupByProject/Update",
      deactivate: (projectGroupByProjectId: Identifier) =>
        `/api/ProjectGroupService/ProjectGroupByProject/Deactivate/${projectGroupByProjectId}`,
    },
    groupWiseStudent: {
      page: "/api/ProjectGroupService/GroupWiseStudent/Page",
      primaryKey: (groupWiseStudentId: Identifier) =>
        `/api/ProjectGroupService/GroupWiseStudent/${groupWiseStudentId}`,
      view: (groupWiseStudentId: Identifier) =>
        `/api/ProjectGroupService/GroupWiseStudent/View/${groupWiseStudentId}`,
      create: "/api/ProjectGroupService/GroupWiseStudent/Create",
      update: "/api/ProjectGroupService/GroupWiseStudent/Update",
      deactivate: (groupWiseStudentId: Identifier) =>
        `/api/ProjectGroupService/GroupWiseStudent/Deactivate/${groupWiseStudentId}`,
    },
  },
} as const;
