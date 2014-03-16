RMCycler
=============

RMCycler (RainmeterCycler) is a simple command line utility that allows users to cycle through their saved [Rainmeter](http://www.rainmeter.net) layouts. Essentially, all it does is keep track of the layouts that have already been cycled through, and feeds Rainmeter the next layout to switch to. It works by calling the Rainmeter executable with the `!Loadlayout` bang, and running it will change the active layout once. RMCycler does not run in the background, or have any scheduling functionality of its own. It is intended to be run at regular intervals, ideally by something such as the Windows Task Scheduler. By using .ini files, users can configure the manner in which RMCycler cycles through layouts. The ability to create multiple configurations helps to improve the scalability and simplicity of adding new layouts to a rotation.


#####Note:
Rainmeter version 3.0.2 (r2161) was used to develop RMCycler, however it should work as long as Rainmeter's `!Loadlayout` functionality doesn't change, and layouts are stored as folders (the way they currently are).



User Guide
-
When run from the command line, RMCycler takes one argument, which is the path to a .ini file that contains the RMCycler options. If no argument is given, or if the configuration file is empty/nonexistent, it will select one layout at random from all of the saved layouts (except the @Backup layout). These configurations are completely independent from each other, meaning that the rules set by a configuration only apply to when that configuration file is used to run RMCycler, and have no influence on the times other configurations are used. It is also up to the user to keep these configurations valid, to prevent RMCycler from feeding Rainmeter nonexistent layout names. (In which case Rainmeter will simply do nothing.)  


**Configuration Options**

* **RMPath** - Path to the Rainmeter executable. `C:\Program Files\Rainmeter\Rainmeter.exe` will be used if not specified.
* **LFPath** - Path to your "Layouts" folder. Use this if you are running a portable installation of Rainmeter, and/or your layouts are not located in your AppData folder. 
* **List** - A comma-separated list of the layouts you want to cycle through. If not specified, it will use all your saved layouts (except for @Backup). The order in which they appear here is also relevant for the `InOrder` selection mode.

 Note:
 * You can have repeated layouts in the list.
 * For `Shuffle` and `InOrder` selection modes, **List** will be completely ignored if there are **Remaining** layouts. (See below.) 
* **Mode** - How RMCycler will cycle through the layouts. Options are:
 * `Random` - Any layout from the specified layouts can be selected. (Default)
 * `Shuffle` - Will cycle through each of the layouts once in a random order, before randomly cycling through all of them again.
 * `InOrder` - Will cycle through the layouts in the same order as they appear in **List**. If **List** has no value (meaning you are cycling through all layouts), `InOrder` should cycle through them alphabetically.
* **Remaining** - A comma-separated list of the layouts remaining in the current rotation cycle, used for the `Shuffle` and `InOrder` selection modes. Generally, you won't have to worry about this section since RMCycler will manage it on its own. However, you can delete it or manually modify it as you require.
 
 Note:
As long as **Remaining** is non-empty, it will be used to choose the next layout, and **List** will be ignored. It will be repopulated using the value of **List** only when it becomes empty.


Example Configs
-

#####Example 1:


```ini
[RMCycler]
RMPath = "C:\Program Files\Rainmeter\Rainmeter.exe"
Mode = Random

;For a standard Rainmeter installation, this config makes no difference from no config at all

```

#####Example 2:

```ini
[RMCycler]
List = layout1,layout1,layout1,layout2,layout3
Mode = Shuffle

;layout1 will appear three times as often as the other layouts

```

#####Example 3:

```ini
[RMCycler]
List = layout1,layout1,layout1,layout2,layout3
Mode = Shuffle
Remaining = layout4,layout4,layout4

;layout4 will appear the first three times, but never again after that

```
