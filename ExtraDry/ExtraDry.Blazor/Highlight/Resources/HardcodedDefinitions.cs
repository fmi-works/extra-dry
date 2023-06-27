﻿namespace ExtraDry.Highlight;

internal static class HardcodedDefinitions {

    public const string Xml = @"
<?xml version=""1.0"" encoding=""utf-8""?>
<definitions>
  <definition name=""Blazor"" caseSensitive=""true"">
    <default>
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""black"" backColor=""transparent""/>
    </default>
    <pattern name=""HtmlComment"" type=""block"" beginsWith=""&amp;lt;!--"" endsWith=""--&amp;gt;"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
    <pattern name=""Markup"" type=""markup"" highlightAttributes=""true"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""maroon"" backColor=""transparent"">
        <bracketStyle foreColor=""blue"" backColor=""transparent""/>
        <attributeNameStyle foreColor=""red"" backColor=""transparent""/>
        <attributeValueStyle foreColor=""blue"" backColor=""transparent""/>
      </font>
    </pattern>
    <pattern name=""Directive"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""purple"" backColor=""transparent""/>
      <word>@code</word>
      <word>@inherits</word>
      <word>@inject</word>
      <word>@implements</word>
    </pattern>
    <pattern name=""Namespace"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>namespace</word>
      <word>using</word>
    </pattern>
    <pattern name=""MethodParameter"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>params</word>
      <word>ref</word>
      <word>out</word>
    </pattern>
    <pattern name=""Statement"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>if</word>
      <word>else</word>
      <word>switch</word>
      <word>case</word>
      <word>do</word>
      <word>for</word>
      <word>foreach</word>
      <word>in</word>
      <word>while</word>
      <word>break</word>
      <word>continue</word>
      <word>goto</word>
      <word>return</word>
      <word>try</word>
      <word>throw</word>
      <word>catch</word>
      <word>finally</word>
      <word>checked</word>
      <word>unchecked</word>
      <word>fixed</word>
      <word>lock</word>
      <word>new</word>
      <word>var</word>
    </pattern>
    <pattern name=""Modifier"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>internal</word>
      <word>private</word>
      <word>protected</word>
      <word>public</word>
      <word>abstract</word>
      <word>const</word>
      <word>default</word>
      <word>event</word>
      <word>extern</word>
      <word>override</word>
      <word>readonly</word>
      <word>sealed</word>
      <word>static</word>
      <word>unsafe</word>
      <word>virtual</word>
      <word>volatile</word>
    </pattern>
    <pattern name=""ValueType"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>void</word>
      <word>bool</word>
      <word>byte</word>
      <word>char</word>
      <word>decimal</word>
      <word>double</word>
      <word>enum</word>
      <word>float</word>
      <word>int</word>
      <word>long</word>
      <word>sbyte</word>
      <word>short</word>
      <word>struct</word>
      <word>uint</word>
      <word>ulong</word>
      <word>ushort</word>
    </pattern>
    <pattern name=""ReferenceType"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>class</word>
      <word>interface</word>
      <word>delegate</word>
      <word>object</word>
      <word>string</word>
    </pattern>
    <pattern name=""Operator"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""red"" backColor=""transparent""/>
      <word>+</word>
      <word>-</word>
      <word>=</word>
      <word>%</word>
      <word>*</word>
      <word>/</word>
      <word>&amp;</word>
      <word>|</word>
    </pattern>
    <pattern name=""String"" type=""block"" beginsWith=""&amp;quot;"" endsWith=""&amp;quot;"" escapesWith=""\"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""#666666"" backColor=""transparent""/>
    </pattern>
    <pattern name=""VerbatimString"" type=""block"" beginsWith=""@&amp;quot;"" endsWith=""&amp;quot;"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""#666666"" backColor=""transparent""/>
    </pattern>
    <pattern name=""Comment"" type=""block"" beginsWith=""//"" endsWith=""\n"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
    <pattern name=""MultiLineComment"" type=""block"" beginsWith=""/*"" endsWith=""*/"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
    <pattern name=""PreprocessorDirective"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>#if</word>
      <word>#else</word>
      <word>#elif</word>
      <word>#endif</word>
      <word>#define</word>
      <word>#undef</word>
      <word>#warning</word>
      <word>#error</word>
      <word>#line</word>
      <word>#region</word>
      <word>#endregion</word>
      <word>#pragma</word>
    </pattern>
  </definition>
  <definition name=""C#"" caseSensitive=""true"">
    <default>
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""black"" backColor=""transparent""/>
    </default>
    <pattern name=""Keyword"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>as</word>
      <word>is</word>
      <word>new</word>
      <word>sizeof</word>
      <word>typeof</word>
      <word>true</word>
      <word>false</word>
      <word>stackalloc</word>
      <word>explicit</word>
      <word>implicit</word>
      <word>operator</word>
      <word>base</word>
      <word>this</word>
      <word>null</word>
    </pattern>
    <pattern name=""Namespace"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>namespace</word>
      <word>using</word>
    </pattern>
    <pattern name=""MethodParameter"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>params</word>
      <word>ref</word>
      <word>out</word>
    </pattern>
    <pattern name=""Statement"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>if</word>
      <word>else</word>
      <word>switch</word>
      <word>case</word>
      <word>do</word>
      <word>for</word>
      <word>foreach</word>
      <word>in</word>
      <word>while</word>
      <word>break</word>
      <word>continue</word>
      <word>goto</word>
      <word>return</word>
      <word>try</word>
      <word>throw</word>
      <word>catch</word>
      <word>finally</word>
      <word>checked</word>
      <word>unchecked</word>
      <word>fixed</word>
      <word>lock</word>
    </pattern>
    <pattern name=""Modifier"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>internal</word>
      <word>private</word>
      <word>protected</word>
      <word>public</word>
      <word>abstract</word>
      <word>const</word>
      <word>default</word>
      <word>event</word>
      <word>extern</word>
      <word>override</word>
      <word>readonly</word>
      <word>sealed</word>
      <word>static</word>
      <word>unsafe</word>
      <word>virtual</word>
      <word>volatile</word>
    </pattern>
    <pattern name=""ValueType"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>void</word>
      <word>bool</word>
      <word>byte</word>
      <word>char</word>
      <word>decimal</word>
      <word>double</word>
      <word>enum</word>
      <word>float</word>
      <word>int</word>
      <word>long</word>
      <word>sbyte</word>
      <word>short</word>
      <word>struct</word>
      <word>uint</word>
      <word>ulong</word>
      <word>ushort</word>
    </pattern>
    <pattern name=""ReferenceType"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>class</word>
      <word>interface</word>
      <word>delegate</word>
      <word>object</word>
      <word>string</word>
    </pattern>
    <pattern name=""Operator"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""red"" backColor=""transparent""/>
      <word>+</word>
      <word>-</word>
      <word>=</word>
      <word>%</word>
      <word>*</word>
      <word>/</word>
      <word>&amp;</word>
      <word>|</word>
    </pattern>
    <pattern name=""String"" type=""block"" beginsWith=""&amp;quot;"" endsWith=""&amp;quot;"" escapesWith=""\"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""#666666"" backColor=""transparent""/>
    </pattern>
    <pattern name=""VerbatimString"" type=""block"" beginsWith=""@&amp;quot;"" endsWith=""&amp;quot;"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""#666666"" backColor=""transparent""/>
    </pattern>
    <pattern name=""Comment"" type=""block"" beginsWith=""//"" endsWith=""\n"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
    <pattern name=""MultiLineComment"" type=""block"" beginsWith=""/*"" endsWith=""*/"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
    <pattern name=""PreprocessorDirective"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>#if</word>
      <word>#else</word>
      <word>#elif</word>
      <word>#endif</word>
      <word>#define</word>
      <word>#undef</word>
      <word>#warning</word>
      <word>#error</word>
      <word>#line</word>
      <word>#region</word>
      <word>#endregion</word>
      <word>#pragma</word>
    </pattern>
  </definition>
  <definition name=""HTML"" caseSensitive=""false"">
    <default>
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""black"" backColor=""transparent""/>
    </default>
    <pattern name=""MultiLineComment"" type=""block"" beginsWith=""&amp;lt;!--"" endsWith=""--&amp;gt;"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
    <pattern name=""Markup"" type=""markup"" highlightAttributes=""true"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""maroon"" backColor=""transparent"">
        <bracketStyle foreColor=""blue"" backColor=""transparent""/>
        <attributeNameStyle foreColor=""red"" backColor=""transparent""/>
        <attributeValueStyle foreColor=""blue"" backColor=""transparent""/>
      </font>
    </pattern>
  </definition>
  <definition name=""JavaScript"" caseSensitive=""true"">
    <default>
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""black"" backColor=""transparent""/>
    </default>
    <pattern name=""Function"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""purple"" backColor=""transparent""/>
      <word>GetObject</word>
      <word>ScriptEngine</word>
      <word>ScriptEngineBuildVersion</word>
      <word>ScriptEngineMajorVersion</word>
      <word>ScriptEngineMinorVersion</word>
    </pattern>
    <pattern name=""Method"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>abs</word>
      <word>acos</word>
      <word>anchor</word>
      <word>apply</word>
      <word>asin</word>
      <word>atan</word>
      <word>atan2</word>
      <word>atEnd</word>
      <word>big</word>
      <word>blink</word>
      <word>bold</word>
      <word>call</word>
      <word>ceil</word>
      <word>charAt</word>
      <word>charCodeAt</word>
      <word>compile</word>
      <word>concat</word>
      <word>cos</word>
      <word>decodeURI</word>
      <word>decodeURIComponent</word>
      <word>dimensions</word>
      <word>encodeURI</word>
      <word>encodeURIComponent</word>
      <word>escape</word>
      <word>eval</word>
      <word>exec</word>
      <word>exp</word>
      <word>fixed</word>
      <word>floor</word>
      <word>fontcolor</word>
      <word>fontsize</word>
      <word>fromCharCode</word>
      <word>getDate</word>
      <word>getDay</word>
      <word>getFullYear</word>
      <word>getHours</word>
      <word>getItem</word>
      <word>getMilliseconds</word>
      <word>getMinutes</word>
      <word>getMonth</word>
      <word>getSeconds</word>
      <word>getTime</word>
      <word>getTimezoneOffset</word>
      <word>getUTCDate</word>
      <word>getUTCDay</word>
      <word>getUTCFullYear</word>
      <word>getUTCHours</word>
      <word>getUTCMilliseconds</word>
      <word>getUTCMinutes</word>
      <word>getUTCMonth</word>
      <word>getUTCSeconds</word>
      <word>getVarDate</word>
      <word>getYear</word>
      <word>hasOwnProperty</word>
      <word>indexOf</word>
      <word>isFinite</word>
      <word>isNaN</word>
      <word>isPrototypeOf</word>
      <word>italics</word>
      <word>item</word>
      <word>join</word>
      <word>lastIndexOf</word>
      <word>lbound</word>
      <word>link</word>
      <word>localeCompare</word>
      <word>log</word>
      <word>match</word>
      <word>max</word>
      <word>min</word>
      <word>moveFirst</word>
      <word>moveNext</word>
      <word>parse</word>
      <word>parseFloat</word>
      <word>parseInt</word>
      <word>pop</word>
      <word>pow</word>
      <word>push</word>
      <word>random</word>
      <word>replace</word>
      <word>reverse</word>
      <word>round</word>
      <word>search</word>
      <word>setDate</word>
      <word>setFullYear</word>
      <word>setHours</word>
      <word>setMilliseconds</word>
      <word>setMinutes</word>
      <word>setMonth</word>
      <word>setSeconds</word>
      <word>setTime</word>
      <word>setUTCDate</word>
      <word>setUTCFullYear</word>
      <word>setUTCHours</word>
      <word>setUTCMilliseconds</word>
      <word>setUTCMinutes</word>
      <word>setUTCMonth</word>
      <word>setUTCSeconds</word>
      <word>setYear</word>
      <word>shift</word>
      <word>sin</word>
      <word>slice</word>
      <word>small</word>
      <word>sort</word>
      <word>splice</word>
      <word>split</word>
      <word>sqrt</word>
      <word>strike</word>
      <word>sub</word>
      <word>substr</word>
      <word>substring</word>
      <word>sup</word>
      <word>tan</word>
      <word>test</word>
      <word>toArray</word>
      <word>toDateString</word>
      <word>toExponential</word>
      <word>toFixed</word>
      <word>toGMTString</word>
      <word>toLocaleDateString</word>
      <word>toLocaleLowerCase</word>
      <word>toLocaleString</word>
      <word>toLocaleTimeString</word>
      <word>toLocaleUpperCase</word>
      <word>toLowerCase</word>
      <word>toPrecision</word>
      <word>toString</word>
      <word>toTimeString</word>
      <word>toUpperCase</word>
      <word>toUTCString</word>
      <word>ubound</word>
      <word>unescape</word>
      <word>unshift</word>
      <word>UTC</word>
      <word>valueOf</word>
      <word>write</word>
      <word>writeln</word>
    </pattern>
    <pattern name=""Object"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""red"" backColor=""transparent""/>
      <word>ActiveXObject</word>
      <word>Array</word>
      <word>arguments</word>
      <word>Boolean</word>
      <word>Date</word>
      <word>Debug</word>
      <word>Enumerator</word>
      <word>Error</word>
      <word>Function</word>
      <word>Global</word>
      <word>Math</word>
      <word>Number</word>
      <word>Object</word>
      <word>RegExp</word>
      <word>String</word>
      <word>VBArray</word>
    </pattern>
    <pattern name=""Statement"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>if</word>
      <word>else</word>
      <word>switch</word>
      <word>case</word>
      <word>do</word>
      <word>for</word>
      <word>function</word>
      <word>in</word>
      <word>while</word>
      <word>break</word>
      <word>continue</word>
      <word>return</word>
      <word>try</word>
      <word>throw</word>
      <word>catch</word>
      <word>finally</word>
      <word>checked</word>
      <word>unchecked</word>
      <word>fixed</word>
      <word>lock</word>
      <word>this</word>
      <word>var</word>
      <word>with</word>
      <word>@cc_on</word>
      <word>@if</word>
      <word>@set</word>
      <word>@elif</word>
      <word>@else</word>
      <word>@end</word>
    </pattern>
    <pattern name=""Operator"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""red"" backColor=""transparent""/>
      <word>+</word>
      <word>-</word>
      <word>=</word>
      <word>%</word>
      <word>*</word>
      <word>/</word>
      <word>&amp;</word>
      <word>|</word>
    </pattern>
    <pattern name=""Property"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""red"" backColor=""transparent""/>
      <word>arguments</word>
      <word>callee</word>
      <word>caller</word>
      <word>constructor</word>
      <word>description</word>
      <word>E</word>
      <word>global</word>
      <word>ignoreCase</word>
      <word>index</word>
      <word>Infinity</word>
      <word>input</word>
      <word>lastIndex</word>
      <word>lastParent</word>
      <word>leftContext</word>
      <word>length</word>
      <word>LN2</word>
      <word>LN10</word>
      <word>LOG2E</word>
      <word>LOG10E</word>
      <word>MAX</word>
      <word>message</word>
      <word>MIN</word>
      <word>multiline</word>
      <word>name</word>
      <word>NaN</word>
      <word>NEGATIVE_INFINITY</word>
      <word>number</word>
      <word>PI</word>
      <word>POSITIVE_INFINITY</word>
      <word>propertyIsEnumerable</word>
      <word>prototype</word>
      <word>rightContext</word>
      <word>source</word>
      <word>SQRT1_2</word>
      <word>SQRT2</word>
      <word>undefined</word>
    </pattern>
    <pattern name=""String"" type=""block"" beginsWith=""&quot;"" endsWith=""&quot;"" escapesWith=""\"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""#666666"" backColor=""transparent""/>
    </pattern>
    <pattern name=""Comment"" type=""block"" beginsWith=""//"" endsWith=""\n"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
    <pattern name=""MultiLineComment"" type=""block"" beginsWith=""/*"" endsWith=""*/"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
  </definition>
  <definition name=""SQL"" caseSensitive=""false"">
    <default>
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""black"" backColor=""transparent""/>
    </default>
    <pattern name=""SysTable"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""lawngreen"" backColor=""transparent""/>
      <word>sysaltfiles</word>
      <word>syslockinfo</word>
      <word>syscacheobjects</word>
      <word>syslogins</word>
      <word>sysxlogins</word>
      <word>syscharsets</word>
      <word>sysmessages</word>
      <word>sysconfigures</word>
      <word>sysoledbusers</word>
      <word>syscurconfigs</word>
      <word>sysperfinfo</word>
      <word>sysdatabases</word>
      <word>sysprocesses</word>
      <word>sysdevices</word>
      <word>sysremotelogins</word>
      <word>syslanguages</word>
      <word>sysservers</word>
      <word>syscolumns</word>
      <word>sysindexkeys</word>
      <word>syscomments</word>
      <word>sysmembers</word>
      <word>sysconstraints</word>
      <word>sysremote_column_privileges</word>
      <word>sysobjects</word>
      <word>sysdepends</word>
      <word>syspermissions</word>
      <word>sysfilegroups</word>
      <word>sysprotects</word>
      <word>sysfiles</word>
      <word>sysreferences</word>
      <word>sysforeignkeys</word>
      <word>sysTypes</word>
      <word>sysfulltextcatalogs</word>
      <word>sysusers</word>
      <word>sysindexes</word>
      <word>sysalerts</word>
      <word>sysjobsteps</word>
      <word>syscategories</word>
      <word>sysnotifications</word>
      <word>sysdownloadlist</word>
      <word>sysoperators</word>
      <word>sysjobhistory</word>
      <word>systargetservergroupmembers</word>
      <word>sysjobs</word>
      <word>systargetservergroups</word>
      <word>sysjobschedules</word>
      <word>systargetservers</word>
      <word>sysjobservers</word>
      <word>systaskids</word>
    </pattern>
    <pattern name=""SysProc"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""brown"" backColor=""transparent""/>
      <word>sp_bindsession</word>
      <word>sp_column_privileges_ex</word>
      <word>sp_createorphan</word>
      <word>sp_cursor</word>
      <word>sp_cursorclose</word>
      <word>sp_cursorexecute</word>
      <word>sp_cursorfetch</word>
      <word>sp_cursoropen</word>
      <word>sp_cursoroption</word>
      <word>sp_cursorprepare</word>
      <word>sp_cursorprepexec</word>
      <word>sp_cursorunprepare</word>
      <word>sp_droporphans</word>
      <word>sp_execute</word>
      <word>sp_executesql</word>
      <word>sp_fulltext_getdata</word>
      <word>sp_getbindtoken</word>
      <word>sp_GetMBCSCharLen</word>
      <word>sp_getschemalock</word>
      <word>sp_IsMBCSLeadByte</word>
      <word>sp_MSgetversion</word>
      <word>sp_OACreate</word>
      <word>sp_OADestroy</word>
      <word>sp_OAGetErrorInfo</word>
      <word>sp_OAGetProperty</word>
      <word>sp_OAMethod</word>
      <word>sp_OASetProperty</word>
      <word>sp_OAStop</word>
      <word>sp_prepare</word>
      <word>sp_prepexec</word>
      <word>sp_prepexecrpc</word>
      <word>sp_refreshview</word>
      <word>sp_releaseschemalock</word>
      <word>sp_replcmds</word>
      <word>sp_replcounters</word>
      <word>sp_repldone</word>
      <word>sp_replflush</word>
      <word>sp_replincrementlsn</word>
      <word>sp_replpostcmd</word>
      <word>sp_replpostschema</word>
      <word>sp_replpostsyncstatus</word>
      <word>sp_replsendtoqueue</word>
      <word>sp_replsetoriginator</word>
      <word>sp_replsetsyncstatus</word>
      <word>sp_repltrans</word>
      <word>sp_replupdateschema</word>
      <word>sp_replwritetovarbin</word>
      <word>sp_reset_connection</word>
      <word>sp_resyncexecute</word>
      <word>sp_resyncexecutesql</word>
      <word>sp_resyncprepare</word>
      <word>sp_resyncuniquetable</word>
      <word>sp_sdidebug</word>
      <word>sp_trace_create</word>
      <word>sp_trace_generateevent</word>
      <word>sp_trace_setevent</word>
      <word>sp_trace_setfilter</word>
      <word>sp_trace_setstatus</word>
      <word>sp_unprepare</word>
      <word>sp_xml_preparedocument</word>
      <word>sp_xml_removedocument</word>
      <word>xp_adsirequest</word>
      <word>xp_availablemedia</word>
      <word>xp_cleanupwebtask</word>
      <word>xp_cmdshell</word>
      <word>xp_controlqueueservice</word>
      <word>xp_convertwebtask</word>
      <word>xp_createprivatequeue</word>
      <word>xp_createqueue</word>
      <word>xp_decodequeuecmd</word>
      <word>xp_deletemail</word>
      <word>xp_deleteprivatequeue</word>
      <word>xp_deletequeue</word>
      <word>xp_dirtree</word>
      <word>xp_displayparamstmt</word>
      <word>xp_displayqueuemesgs</word>
      <word>xp_dropwebtask</word>
      <word>xp_dsninfo</word>
      <word>xp_enum_activescriptengines</word>
      <word>xp_enum_oledb_providers</word>
      <word>xp_enumcodepages</word>
      <word>xp_enumdsn</word>
      <word>xp_enumerrorlogs</word>
      <word>xp_enumgroups</word>
      <word>xp_eventlog</word>
      <word>xp_execresultset</word>
      <word>xp_fileexist</word>
      <word>xp_findnextmsg</word>
      <word>xp_fixeddrives</word>
      <word>xp_get_mapi_default_profile</word>
      <word>xp_get_mapi_profiles</word>
      <word>xp_get_tape_devices</word>
      <word>xp_GetAdminGroupName</word>
      <word>xp_getfiledetails</word>
      <word>xp_getnetname</word>
      <word>xp_getprotocoldllinfo</word>
      <word>xp_instance_regaddmultistring</word>
      <word>xp_instance_regdeletekey</word>
      <word>xp_instance_regdeletevalue</word>
      <word>xp_instance_regenumkeys</word>
      <word>xp_instance_regenumvalues</word>
      <word>xp_instance_regread</word>
      <word>xp_instance_regremovemultistring</word>
      <word>xp_instance_regwrite</word>
      <word>xp_intersectbitmaps</word>
      <word>xp_IsNTAdmin</word>
      <word>xp_logevent</word>
      <word>xp_loginconfig</word>
      <word>xp_makecab</word>
      <word>xp_makewebtask</word>
      <word>xp_mapdown_bitmap</word>
      <word>xp_mergelineages</word>
      <word>xp_mergexpusage</word>
      <word>xp_MSADEnabled</word>
      <word>xp_MSADSIObjReg</word>
      <word>xp_MSADSIObjRegDB</word>
      <word>xp_MSADSIReg</word>
      <word>xp_MSFullText</word>
      <word>xp_MSLocalSystem</word>
      <word>xp_MSnt2000</word>
      <word>xp_MSplatform</word>
      <word>xp_msver</word>
      <word>xp_msx_enlist</word>
      <word>xp_ntsec_enumdomains</word>
      <word>xp_oledbinfo</word>
      <word>xp_ORbitmap</word>
      <word>xp_peekqueue</word>
      <word>xp_printstatements</word>
      <word>xp_prop_oledb_provider</word>
      <word>xp_proxiedmetadata</word>
      <word>xp_qv</word>
      <word>xp_readerrorlog</word>
      <word>xp_readmail</word>
      <word>xp_readpkfromqueue</word>
      <word>xp_readpkfromvarbin</word>
      <word>xp_readwebtask</word>
      <word>xp_regaddmultistring</word>
      <word>xp_regdeletekey</word>
      <word>xp_regdeletevalue</word>
      <word>xp_regenumkeys</word>
      <word>xp_regenumvalues</word>
      <word>xp_regread</word>
      <word>xp_regremovemultistring</word>
      <word>xp_regwrite</word>
      <word>xp_repl_convert_encrypt</word>
      <word>xp_repl_encrypt</word>
      <word>xp_repl_help_connect</word>
      <word>xp_replproberemsrv</word>
      <word>xp_resetqueue</word>
      <word>xp_runwebtask</word>
      <word>xp_sendmail</word>
      <word>xp_servicecontrol</word>
      <word>xp_SetSQLSecurity</word>
      <word>xp_showcolv</word>
      <word>xp_showlineage</word>
      <word>xp_sprintf</word>
      <word>xp_sqlagent_enum_jobs</word>
      <word>xp_sqlagent_is_starting</word>
      <word>xp_sqlagent_monitor</word>
      <word>xp_sqlagent_notify</word>
      <word>xp_sqlagent_param</word>
      <word>xp_sqlagent_proxy_account</word>
      <word>xp_sqlmaint</word>
      <word>xp_sscanf</word>
      <word>xp_startmail</word>
      <word>xp_stopmail</word>
      <word>xp_subdirs</word>
      <word>xp_terminate_process</word>
      <word>xp_test_mapi_profile</word>
      <word>xp_unc_to_drive</word>
      <word>xp_unpackcab</word>
      <word>xp_updateFTSSQLAccount</word>
      <word>xp_userlock</word>
      <word>xp_varbintohexstr</word>
    </pattern>
    <pattern name=""GlobalVariable"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""fuchsia"" backColor=""transparent""/>
      <word>@@DATEFIRST</word>
      <word>@@OPTIONS</word>
      <word>@@DBTS</word>
      <word>@@REMSERVER</word>
      <word>@@LANGID</word>
      <word>@@SERVERName</word>
      <word>@@LANGUAGE</word>
      <word>@@SERVICEName</word>
      <word>@@LOCK_TIMEOUT</word>
      <word>@@SPID</word>
      <word>@@MAX_CONNECTIONS</word>
      <word>@@TEXTSIZE</word>
      <word>@@MAX_PRECISION</word>
      <word>@@VERSION</word>
      <word>@@NESTLEVEL</word>
      <word>@@CURSOR_ROWS</word>
      <word>@@FETCH_STATUS</word>
      <word>@@PROCID</word>
      <word>@@IDENTITY</word>
      <word>@@ROWCOUNT</word>
      <word>@@ERROR</word>
      <word>@@TRANCOUNT</word>
      <word>@@CONNECTIONS</word>
      <word>@@PACK_RECEIVED</word>
      <word>@@CPU_BUSY</word>
      <word>@@PACK_SENT</word>
      <word>@@TIMETICKS</word>
      <word>@@IDLE</word>
      <word>@@TOTAL_ERRORS</word>
      <word>@@IO_BUSY</word>
      <word>@@TOTAL_READ</word>
      <word>@@PACKET_ERRORS</word>
      <word>@@TOTAL_WRITE</word>
    </pattern>
    <pattern name=""ReservedKeyword"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>sysname</word>
      <word>ADD</word>
      <word>EXCEPT</word>
      <word>PERCENT</word>
      <word>EXEC</word>
      <word>PLAN</word>
      <word>ALTER</word>
      <word>EXECUTE</word>
      <word>PRECISION</word>
      <word>PRIMARY</word>
      <word>EXIT</word>
      <word>PRINT</word>
      <word>AS</word>
      <word>FETCH</word>
      <word>NOCOUNT</word>
      <word>PROC</word>
      <word>ASC</word>
      <word>FILE</word>
      <word>PROCEDURE</word>
      <word>AUTHORIZATION</word>
      <word>FILLFACTOR</word>
      <word>PUBLIC</word>
      <word>BACKUP</word>
      <word>FOR</word>
      <word>RAISE</word>
      <word>RAISERROR</word>
      <word>BEGIN</word>
      <word>FOREIGN</word>
      <word>READ</word>
      <word>FREETEXT</word>
      <word>READTEXT</word>
      <word>BREAK</word>
      <word>FREETEXTTABLE</word>
      <word>RECONFIGURE</word>
      <word>BROWSE</word>
      <word>FROM</word>
      <word>REFERENCES</word>
      <word>BULK</word>
      <word>FULL</word>
      <word>REPLICATION</word>
      <word>BY</word>
      <word>FUNCTION</word>
      <word>RESTORE</word>
      <word>CASCADE</word>
      <word>GOTO</word>
      <word>RESTRICT</word>
      <word>GRANT</word>
      <word>RETURN</word>
      <word>CHECK</word>
      <word>GROUP</word>
      <word>REVOKE</word>
      <word>CHECKPOINT</word>
      <word>HAVING</word>
      <word>CLOSE</word>
      <word>HOLDLOCK</word>
      <word>ROLLBACK</word>
      <word>CLUSTERED</word>
      <word>IDENTITY</word>
      <word>IDENTITY_INSERT</word>
      <word>ROWGUIDCOL</word>
      <word>COLLATE</word>
      <word>IDENTITYCOL</word>
      <word>RULE</word>
      <word>COLUMN</word>
      <word>IF</word>
      <word>SAVE</word>
      <word>COMMIT</word>
      <word>SCHEMA</word>
      <word>COMPUTE</word>
      <word>INDEX</word>
      <word>SELECT</word>
      <word>CONSTRAINT</word>
      <word>INNER</word>
      <word>CONTAINS</word>
      <word>INSERT</word>
      <word>SET</word>
      <word>CONTAINSTABLE</word>
      <word>INTERSECT</word>
      <word>SETUSER</word>
      <word>CONTINUE</word>
      <word>INTO</word>
      <word>SHUTDOWN</word>
      <word>IS</word>
      <word>CREATE</word>
      <word>STATISTICS</word>
      <word>KEY</word>
      <word>CURRENT</word>
      <word>KILL</word>
      <word>TABLE</word>
      <word>CURRENT_DATE</word>
      <word>TEXTSIZE</word>
      <word>CURRENT_TIME</word>
      <word>THEN</word>
      <word>LINENO</word>
      <word>TO</word>
      <word>LOAD</word>
      <word>TOP</word>
      <word>CURSOR</word>
      <word>NATIONAL</word>
      <word>TRAN</word>
      <word>DATABASE</word>
      <word>NOCHECK</word>
      <word>TRANSACTION</word>
      <word>DBCC</word>
      <word>NONCLUSTERED</word>
      <word>TRIGGER</word>
      <word>DEALLOCATE</word>
      <word>TRUNCATE</word>
      <word>DECLARE</word>
      <word>TSEQUAL</word>
      <word>DEFAULT</word>
      <word>UNION</word>
      <word>DELETE</word>
      <word>OF</word>
      <word>UNIQUE</word>
      <word>DENY</word>
      <word>OFF</word>
      <word>UPDATE</word>
      <word>DESC</word>
      <word>OFFSETS</word>
      <word>UPDATETEXT</word>
      <word>DISK</word>
      <word>ON</word>
      <word>USE</word>
      <word>DISTINCT</word>
      <word>OPEN</word>
      <word>DISTRIBUTED</word>
      <word>OPENDATASOURCE</word>
      <word>VALUES</word>
      <word>DOUBLE</word>
      <word>OPENQUERY</word>
      <word>VARYING</word>
      <word>DROP</word>
      <word>OPENROWSET</word>
      <word>VIEW</word>
      <word>DUMMY</word>
      <word>OPENXML</word>
      <word>WAITFOR</word>
      <word>DUMP</word>
      <word>OPTION</word>
      <word>WHEN</word>
      <word>ELSE</word>
      <word>WHERE</word>
      <word>END</word>
      <word>ORDER</word>
      <word>WHILE</word>
      <word>ERRLVL</word>
      <word>WITH</word>
      <word>ESCAPE</word>
      <word>OVER</word>
      <word>WRITETEXT</word>
      <word>QUOTED_IDENTIFIER</word>
      <word>ANSI_NULLS</word>
      <word>OUTPUT</word>
      <word>OUT</word>
    </pattern>
    <pattern name=""Function"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""fuchsia"" backColor=""transparent""/>
      <word>AVG</word>
      <word>MAX</word>
      <word>BINARY_CHECKSUM</word>
      <word>MIN</word>
      <word>CHECKSUM</word>
      <word>SUM</word>
      <word>CHECKSUM_AGG</word>
      <word>STDEV</word>
      <word>COUNT</word>
      <word>STDEVP</word>
      <word>COUNT_BIG</word>
      <word>VAR</word>
      <word>GROUPING</word>
      <word>VARP</word>
      <word>CURSOR_STATUS</word>
      <word>DATEADD</word>
      <word>DATEDIFF</word>
      <word>DATEName</word>
      <word>DATEPART</word>
      <word>DAY</word>
      <word>GETDATE</word>
      <word>GETUTCDATE</word>
      <word>MONTH</word>
      <word>YEAR</word>
      <word>ABS</word>
      <word>DEGREES</word>
      <word>RAND</word>
      <word>ACOS</word>
      <word>EXP</word>
      <word>ROUND</word>
      <word>ASIN</word>
      <word>FLOOR</word>
      <word>SIGN</word>
      <word>ATAN</word>
      <word>LOG</word>
      <word>SIN</word>
      <word>ATN2</word>
      <word>LOG10</word>
      <word>SQUARE</word>
      <word>CEILING</word>
      <word>PI</word>
      <word>SQRT</word>
      <word>COS</word>
      <word>POWER</word>
      <word>TAN</word>
      <word>COT</word>
      <word>RADIANS</word>
      <word>COL_LENGTH</word>
      <word>COL_Name</word>
      <word>FULLTEXTCATALOGPROPERTY</word>
      <word>COLUMNPROPERTY</word>
      <word>FULLTEXTSERVICEPROPERTY</word>
      <word>DATABASEPROPERTY</word>
      <word>INDEX_COL</word>
      <word>DATABASEPROPERTYEX</word>
      <word>INDEXKEY_PROPERTY</word>
      <word>DB_ID</word>
      <word>INDEXPROPERTY</word>
      <word>DB_Name</word>
      <word>OBJECT_ID</word>
      <word>FILE_ID</word>
      <word>OBJECT_Name</word>
      <word>FILE_Name</word>
      <word>OBJECTPROPERTY</word>
      <word>FILEGROUP_ID</word>
      <word>FILEGROUP_Name</word>
      <word>SQL_VARIANT_PROPERTY</word>
      <word>FILEGROUPPROPERTY</word>
      <word>TypePROPERTY</word>
      <word>FILEPROPERTY</word>
      <word>IS_SRVROLEMEMBER</word>
      <word>SUSER_SID</word>
      <word>SUSER_SName</word>
      <word>USER_ID</word>
      <word>HAS_DBACCESS</word>
      <word>USER</word>
      <word>IS_MEMBER</word>
      <word>ASCII</word>
      <word>NCHAR</word>
      <word>SOUNDEX</word>
      <word>CHAR</word>
      <word>PATINDEX</word>
      <word>SPACE</word>
      <word>CHARINDEX</word>
      <word>REPLACE</word>
      <word>STR</word>
      <word>DIFFERENCE</word>
      <word>QUOTEName</word>
      <word>STUFF</word>
      <word>LEFT</word>
      <word>REPLICATE</word>
      <word>SUBSTRING</word>
      <word>LEN</word>
      <word>REVERSE</word>
      <word>UNICODE</word>
      <word>LOWER</word>
      <word>RIGHT</word>
      <word>UPPER</word>
      <word>LTRIM</word>
      <word>RTRIM</word>
      <word>APP_Name</word>
      <word>CASE</word>
      <word>CAST</word>
      <word>CONVERT</word>
      <word>COALESCE</word>
      <word>COLLATIONPROPERTY</word>
      <word>CURRENT_TIMESTAMP</word>
      <word>CURRENT_USER</word>
      <word>DATALENGTH</word>
      <word>FORMATMESSAGE</word>
      <word>GETANSINULL</word>
      <word>HOST_ID</word>
      <word>HOST_Name</word>
      <word>IDENT_CURRENT</word>
      <word>IDENT_INCRIDENT_SEED</word>
      <word>IDENTITY</word>
      <word>ISDATE</word>
      <word>ISNULL</word>
      <word>ISNUMERIC</word>
      <word>NEWID</word>
      <word>NULLIF</word>
      <word>PARSEName</word>
      <word>PERMISSIONS</word>
      <word>ROWCOUNT_BIG</word>
      <word>SCOPE_IDENTITY</word>
      <word>SERVERPROPERTY</word>
      <word>SESSIONPROPERTY</word>
      <word>SESSION_USER</word>
      <word>STATS_DATE</word>
      <word>SYSTEM_USER</word>
      <word>USER_Name</word>
      <word>PATINDEX</word>
      <word>TEXTPTR</word>
      <word>TEXTVALID</word>
      <word>CASE</word>
      <word>RIGHT</word>
      <word>COALESCE</word>
      <word>SESSION_USER</word>
      <word>CONVERT</word>
      <word>SYSTEM_USER</word>
      <word>LEFT</word>
      <word>CURRENT_TIMESTAMP</word>
      <word>CURRENT_USER</word>
      <word>NULLIF</word>
      <word>USER</word>
    </pattern>
    <pattern name=""DataType"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""blue"" backColor=""transparent""/>
      <word>bigint</word>
      <word>int</word>
      <word>smallint</word>
      <word>tinyint</word>
      <word>bit</word>
      <word>decimal</word>
      <word>numeric</word>
      <word>money</word>
      <word>smallmoney</word>
      <word>float</word>
      <word>real</word>
      <word>datetime</word>
      <word>smalldatetime</word>
      <word>char</word>
      <word>varchar</word>
      <word>text</word>
      <word>nchar</word>
      <word>nvarchar</word>
      <word>ntext</word>
      <word>binary</word>
      <word>varbinary</word>
      <word>image</word>
      <word>cursor</word>
      <word>sql_variant</word>
      <word>table</word>
      <word>timestamp</word>
      <word>uniqueidentifier</word>
    </pattern>
    <pattern name=""Operator"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""silver"" backColor=""transparent""/>
      <word>ALL</word>
      <word>AND</word>
      <word>EXISTS</word>
      <word>ANY</word>
      <word>BETWEEN</word>
      <word>IN</word>
      <word>SOME</word>
      <word>JOIN</word>
      <word>CROSS</word>
      <word>LIKE</word>
      <word>NOT</word>
      <word>NULL</word>
      <word>OR</word>
      <word>OUTER</word>
    </pattern>
    <pattern name=""SystemFunction"" type=""word"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""brown"" backColor=""transparent""/>
      <word>fn_listextendedproperty</word>
      <word>fn_trace_getinfo</word>
      <word>fn_trace_gettable</word>
      <word>fn_trace_geteventinfo</word>
      <word>fn_trace_getfilterinfo</word>
      <word>fn_helpcollations</word>
      <word>fn_servershareddrives</word>
      <word>fn_virtualfilestats</word>
      <word>fn_virtualfilestats</word>
    </pattern>
    <pattern name=""Comment"" type=""block"" beginsWith=""--"" endsWith=""\n"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""teal"" backColor=""transparent""/>
    </pattern>
    <pattern name=""BlockComment"" type=""block"" beginsWith=""/*"" endsWith=""*/"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""teal"" backColor=""transparent""/>
    </pattern>
    <pattern name=""String"" type=""block"" beginsWith=""'"" endsWith=""'"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""red"" backColor=""transparent""/>
    </pattern>
  </definition>
  <definition name=""XML"" caseSensitive=""false"">
    <default>
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""black"" backColor=""transparent""/>
    </default>
    <pattern name=""MultiLineComment"" type=""block"" beginsWith=""&amp;lt;!-"" endsWith=""-&amp;gt;"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""green"" backColor=""transparent""/>
    </pattern>
    <pattern name=""Markup"" type=""markup"" highlightAttributes=""true"">
      <font name=""Courier New"" size=""11"" style=""regular"" foreColor=""maroon"" backColor=""transparent"">
        <bracketStyle foreColor=""blue"" backColor=""transparent""/>
        <attributeNameStyle foreColor=""red"" backColor=""transparent""/>
        <attributeValueStyle foreColor=""blue"" backColor=""transparent""/>
      </font>
    </pattern>
  </definition>
</definitions>
";
}
