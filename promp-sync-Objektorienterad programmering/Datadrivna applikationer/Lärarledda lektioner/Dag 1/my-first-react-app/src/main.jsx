import { StrictMode, useState } from 'react';
import { createRoot } from 'react-dom/client';
import ListOfUsers from './ListOfUsers.jsx';

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <HomePage />
    <ListExample />
    <ListOfUsers />
  </StrictMode>,
);





function HomePage() {
  const name = "World"; 
  return <h1>Hello, {name}!</h1>;
}

// function ListExample() {
//   const [toggled, set_toggled] = useState(false);
//   let items = ["Carl", "John", "Jane"];
  

//   function toggle_toggled() {
//     set_toggled(!toggled);
//     console.log(toggled);
//   }

//   if (toggled) {
//     return (
//       <ul>
//         <button onClick={toggle_toggled}>Switch to map example</button>
//         <li>{items[0]}</li>
//         <li>{items[1]}</li>
//         <li>{items[2]}</li>
//         <li>{items[3]}</li>
//       </ul>
//     );
//   } else {
//     return (
//       <ul>
//         <button onClick={toggle_toggled}>Switch to list example</button>
//         {items.map((item) => (
//           <li key={item}>{item}</li>
//         ))}
//       </ul>
//     );
//   }
// }



function add_one_mod_three(value)
{
  return (value + 1) % 3;
}


function ListExample() {
  const [toggled, set_toggled] = useState(0);
  let items = ["Carl", "John", "Jane"];
  

  function toggle_toggled() {
    //gör det med if satser
    // if (toggled == 0)
    // {
    //   set_toggled(1);
    // }
    // else if (toggled == 1)
    // {
    //   set_toggled(2);
    // }
    // else
    // {
    //   set_toggled(0);
    // }

    //set_toggled(current => (current+1) % 3); // toggled = toggled + 1 % 3
    set_toggled (add_one_mod_three(toggled)); // toggled = add_one_mod_three(toggled);
    console.log(toggled);
  }

  if (toggled) {
    return (
      <ul>
        <button onClick={toggle_toggled}>Switch to 1</button>
        <li>{items[0]}</li>
        <li>{items[1]}</li>
        <li>{items[2]}</li>
        <li>{items[3]}</li>
      </ul>
    );
  } else {
    return (
      <ul>
        <button onClick={toggle_toggled}>Switch 0</button>
        {items.map((item) => (
          <li key={item}>{item}</li>
        ))}
      </ul>
    );
  }
}

