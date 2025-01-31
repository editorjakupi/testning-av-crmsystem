export default function ListOfUsers() {
    
  const users = ["Carl", "John", "Jane"];

  return (
    <ul>
      {users.map((username, index) => (
        <UserProfile 
          key={index} 
          user={username} 
          birthdate="00/00/0000" 
        />
      ))}
    </ul>
  );
}

function UserProfile({ user, birthdate }) {
  return (
    <li>
      <p>Username: {user}</p>
      <p>Birthdate: {birthdate}</p>
    </li>
  );
}


