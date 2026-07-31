1. you don't have to write the private access modifier, it's set to automatic.
2. use public instead of SerializedField
3. Recommended to display even private fields in the inspector, in the "Debug" section
4. If you are using Gizmos, add the "gizmosEnabled" field to the "Debug" section
5. If you are using Debug.Log*, add the "debugEnabled" field to the "Debug" section
6. In the title of the "Settings" section, you should also add the name of the class, since this allows you to track whether the settings belong to the class during inheritance. e.g: Incorrect: "Settings" Correct: "Settings Player". In Inspector: [Settings Entity] -> [Settings Player]
7. Abstractions are preferable to an interface
8. Use RequireComponent to indicate the required dependencies for the component.
9. Use RequireComponent with an abstract class/interface to indicate the need to implement the abstraction.
10. All dependencies and links to other components are listed under the heading "Dependencies"
11. In links, always try to specify the type as accurately as possible, from bad to better: object->GameObject->Monobehaviour->BaseClass->ClassImplementation.
