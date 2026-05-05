import React from "react";
import { Link } from "react-router-dom";

const UserHomePage: React.FC = () => {
  return (
    <div>
      <nav style={{ borderBottom: "1px solid #ccc", padding: "8px 0" }}>
        <ul
          style={{
            listStyle: "none",
            display: "flex",
            gap: "16px",
            margin: 0,
            padding: 0,
          }}
        >
          <li>
            <Link to="/club">Danh sách câu lạc bộ</Link>
          </li>
        </ul>
      </nav>
      <div style={{ padding: "16px" }}>
        <h2>Chào mừng bạn đến trang chủ!</h2>
        {/* Nội dung trang chủ sẽ ở đây */}
      </div>
    </div>
  );
};

export default UserHomePage;
