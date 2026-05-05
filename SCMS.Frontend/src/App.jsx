import LoginPage from "./pages/LoginPage";
import UserHomePage from "./pages/UserHomePage";

import ClubListPage from "./pages/ClubListPage";
import ClubRegisterForm from "./pages/ClubRegisterForm";
import { BrowserRouter, Routes, Route } from "react-router-dom";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/user/home" element={<UserHomePage />} />
        <Route path="/club" element={<ClubListPage />} />
        <Route path="/clubs/:clubId/register" element={<ClubRegisterForm />} />
        <Route path="/club/register" element={<ClubRegisterForm />} />
        {/* ...route khác */}
      </Routes>
    </BrowserRouter>
  );
}
export default App;
