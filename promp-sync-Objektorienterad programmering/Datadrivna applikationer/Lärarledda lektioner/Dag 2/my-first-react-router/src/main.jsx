import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter, Routes, Route, NavLink, Link, useParams } from "react-router";
 
createRoot(document.getElementById("root")).render(
<StrictMode>
<BrowserRouter>
<Routes>
<Route index element={<HomePage/>} />
<Route path="pages/one" element={<PageOne/>} />
<Route path="pages/two" element={<PageTwo/>} />
<Route path="users/:userId" element={<UserProfile/>} />
</Routes>
</BrowserRouter>
</StrictMode>
);
 
function UserProfile()
{
    const users = [{id: "1", name: "User 1"}, {id: "2", name: "User 2"}, {id: "3", name: "User 3"}, {id: "4", name: "User 4"}];
    const {userId} = useParams();  // samma som const params = useParams(), params.userId
    const user = users.find(u => u.id === userId);
    /*
    let user;
    for(const u of users)
    {
        if(u.id === params.userId)
        {
            user = u;
        }
    }
    */
 
    if(!user) {return <main><p>No such user found</p></main>}
 
    return <main>
<p>Welcome, {user.name}!</p>
</main>
}
 
function HomePage()
{
    return <main>
<h1>Home Page</h1>
<NavBar/>
</main>;
}
 
function PageOne()
{
    return <main>
<h1>Page One</h1>
<NavBar/>
</main>;
}
 
function PageTwo()
{
    return <main>
<h1>Page Two</h1>
<NavBar/>
</main>;
}
 
function NavBar()
{
 
    function getActiveClassName(active)
    {
        return active ? "active-link" : "";
    }
 
    return <nav>
<ul>
<li><NavLink className={getActiveClassName} to="/"><button>Home Page</button></NavLink></li>
<li><NavLink className={getActiveClassName} to="/pages/one">Page One</NavLink></li>
<li><NavLink className={getActiveClassName} to="/pages/two">Page Two</NavLink></li>
</ul>
</nav>
}