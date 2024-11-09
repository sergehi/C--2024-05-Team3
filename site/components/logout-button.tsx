"use client";

import { signOut } from "next-auth/react";
import React from "react";
import { LogoutIcon } from "./icons";

const LogoutButton: React.FC = () => {
  const handleLogout = async () => {
    await signOut({ redirect: false });
  };

  return (
    <button
      onClick={handleLogout}
      className="p-2 rounded-full hover:opacity-80 focus:outline-none transition-opacity duration-150 ease-in-out"
      aria-label="Logout"
      title="Выйти"
    >
      <LogoutIcon size={20} />
    </button>
  );
};

export default LogoutButton;
