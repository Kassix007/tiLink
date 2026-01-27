import React from 'react';
import './Header.css';

const Header: React.FC = () => {
    return (
        <header className="header">
            <div className="header-content">
                <h1>Ti Link</h1>
                <nav>
                    <ul className="nav-links">
                        <li><a href="Shorten">Shorten</a></li>
                        <li><a href="Dashboard">Dashboard</a></li>
                        <li><a href="About">About</a></li>
                    </ul>
                </nav>
            </div>
        </header>
    );
};

export default Header;