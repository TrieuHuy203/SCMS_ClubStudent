import React from "react";
import { User } from "../types/user";

interface UserTableProps {
  users: User[];
}

const UserTable: React.FC<UserTableProps> = ({ users }) => (
  <table>
    <thead>
      <tr>
        <th>ID</th>
        <th>Tên</th>
        <th>Email</th>
      </tr>
    </thead>
  <tbody>
  {users.map(u => (
    <tr key={u.id}>
      <td>{u.id}</td>
      <td>{u.fullName}</td>
      <td>{u.email}</td>
    </tr>
  ))}
</tbody>
  </table>
);

export default UserTable;
