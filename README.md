#
<p align="center">
sqlc-gen-csharp
</p>
<p align="center">
<img src="https://github.com/DaredevilOSS/sqlc-gen-csharp/actions/workflows/ci.yml/badge.svg?branch=main" alt="CI">
</p>
<p>sqlc-gen-csharp is a .Net plugin for <a
href="https://github.com/sqlc-dev/sqlc">sqlc</a>.<br/> It leverages the
SQLC plugin system to generate type-safe C# code for SQL queries,
supporting PostgresSQL, MySQL &amp; SQLite via the corresponding driver
or suitable Dapper abstraction.</p>
<h2 id="usage">Usage</h2>
<h3 id="options">Options</h3>
<table>
<colgroup>
<col style="width: 4%" />
<col style="width: 33%" />
<col style="width: 1%" />
<col style="width: 60%" />
</colgroup>
<thead>
<tr class="header">
<th>Option</th>
<th>Possible values</th>
<th>Optional</th>
<th>Info</th>
</tr>
</thead>
<tbody>
<tr class="odd">
<td>overrideDriverVersion</td>
<td>default:<br/> <code>2.3.6</code> for MySqlConnector
(mysql)<br/><code>8.0.3</code> for Npgsql
(postgresql)<br/><code>8.0.10</code> for Microsoft.Data.Sqlite
(sqlite)<br/><br/>values: The desired driver version</td>
<td>Yes</td>
<td>Determines the version of the driver to be used.</td>
</tr>
<tr class="even">
<td>targetFramework</td>
<td>default: <code>net8.0</code><br/>values:
<code>netstandard2.0</code>, <code>netstandard2.1</code>,
<code>net8.0</code></td>
<td>Yes</td>
<td>Determines the target framework for your generated code, meaning the
generated code will be compiled to the specified runtime.<br/>For more
information and help deciding on the right value, refer to the <a
href="https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-1-0">Microsoft
.NET Standard documentation</a>.</td>
</tr>
<tr class="odd">
<td>generateCsproj</td>
<td>default: <code>true</code><br/>values:
<code>false</code>,<code>true</code></td>
<td>Yes</td>
<td>Assists you with the integration of SQLC and csharp by generating a
<code>.csproj</code> file. This converts the generated output to a .dll,
a project that you can easily incorporate into your build process.</td>
</tr>
<tr class="even">
<td>namespaceName</td>
<td>default: the generated project name</td>
<td>Yes</td>
<td>Allows you to override the namespace name to be different than the
project name</td>
</tr>
<tr class="odd">
<td>useDapper</td>
<td>default: <code>false</code><br/>values:
<code>false</code>,<code>true</code></td>
<td>Yes</td>
<td>Enables Dapper as a thin wrapper for the generated code. For more
information, please refer to the <a
href="https://github.com/DapperLib/Dapper">Dapper
documentation</a>.</td>
</tr>
<tr class="even">
<td>overrideDapperVersion</td>
<td>default:<br/> <code>2.1.35</code><br/>values: The desired Dapper
version</td>
<td>Yes</td>
<td>If <code>useDapper</code> is set to <code>true</code>, this option
allows you to override the version of Dapper to be used.</td>
</tr>
</tbody>
</table>
<h1 id="quickstart">Quickstart</h1>
<h2 id="configuration">Configuration</h2>
<div class="sourceCode" id="cb1"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb1-1"><a href="#cb1-1" aria-hidden="true" tabindex="-1"></a><span class="fu">version</span><span class="kw">:</span><span class="at"> </span><span class="st">&quot;2&quot;</span></span>
<span id="cb1-2"><a href="#cb1-2" aria-hidden="true" tabindex="-1"></a><span class="fu">plugins</span><span class="kw">:</span></span>
<span id="cb1-3"><a href="#cb1-3" aria-hidden="true" tabindex="-1"></a><span class="kw">-</span><span class="at"> </span><span class="fu">name</span><span class="kw">:</span><span class="at"> csharp</span></span>
<span id="cb1-4"><a href="#cb1-4" aria-hidden="true" tabindex="-1"></a><span class="at">  </span><span class="fu">wasm</span><span class="kw">:</span></span>
<span id="cb1-5"><a href="#cb1-5" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">url</span><span class="kw">:</span><span class="at"> https://github.com/DaredevilOSS/sqlc-gen-csharp/releases/download/v0.13.0/sqlc-gen-csharp.wasm</span></span>
<span id="cb1-6"><a href="#cb1-6" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">sha256</span><span class="kw">:</span><span class="at"> e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855</span></span>
<span id="cb1-7"><a href="#cb1-7" aria-hidden="true" tabindex="-1"></a><span class="fu">sql</span><span class="kw">:</span></span>
<span id="cb1-8"><a href="#cb1-8" aria-hidden="true" tabindex="-1"></a><span class="co">  # For PostgresSQL</span></span>
<span id="cb1-9"><a href="#cb1-9" aria-hidden="true" tabindex="-1"></a><span class="at">  </span><span class="kw">-</span><span class="at"> </span><span class="fu">schema</span><span class="kw">:</span><span class="at"> schema.sql</span></span>
<span id="cb1-10"><a href="#cb1-10" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">queries</span><span class="kw">:</span><span class="at"> queries.sql</span></span>
<span id="cb1-11"><a href="#cb1-11" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">engine</span><span class="kw">:</span><span class="at"> postgresql</span></span>
<span id="cb1-12"><a href="#cb1-12" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">codegen</span><span class="kw">:</span></span>
<span id="cb1-13"><a href="#cb1-13" aria-hidden="true" tabindex="-1"></a><span class="at">      </span><span class="kw">-</span><span class="at"> </span><span class="fu">plugin</span><span class="kw">:</span><span class="at"> csharp</span></span>
<span id="cb1-14"><a href="#cb1-14" aria-hidden="true" tabindex="-1"></a><span class="at">        </span><span class="fu">out</span><span class="kw">:</span><span class="at"> PostgresDalGen</span></span>
<span id="cb1-15"><a href="#cb1-15" aria-hidden="true" tabindex="-1"></a><span class="co">  # For MySQL</span></span>
<span id="cb1-16"><a href="#cb1-16" aria-hidden="true" tabindex="-1"></a><span class="at">  </span><span class="kw">-</span><span class="at"> </span><span class="fu">schema</span><span class="kw">:</span><span class="at"> schema.sql</span></span>
<span id="cb1-17"><a href="#cb1-17" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">queries</span><span class="kw">:</span><span class="at"> queries.sql</span></span>
<span id="cb1-18"><a href="#cb1-18" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">engine</span><span class="kw">:</span><span class="at"> mysql</span></span>
<span id="cb1-19"><a href="#cb1-19" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">codegen</span><span class="kw">:</span></span>
<span id="cb1-20"><a href="#cb1-20" aria-hidden="true" tabindex="-1"></a><span class="at">      </span><span class="kw">-</span><span class="at"> </span><span class="fu">plugin</span><span class="kw">:</span><span class="at"> csharp</span></span>
<span id="cb1-21"><a href="#cb1-21" aria-hidden="true" tabindex="-1"></a><span class="at">        </span><span class="fu">out</span><span class="kw">:</span><span class="at"> MySqlDalGen</span></span>
<span id="cb1-22"><a href="#cb1-22" aria-hidden="true" tabindex="-1"></a><span class="co">  # For SQLite</span></span>
<span id="cb1-23"><a href="#cb1-23" aria-hidden="true" tabindex="-1"></a><span class="at">  </span><span class="kw">-</span><span class="at"> </span><span class="fu">schema</span><span class="kw">:</span><span class="at"> schema.sql</span></span>
<span id="cb1-24"><a href="#cb1-24" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">queries</span><span class="kw">:</span><span class="at"> queries.sql</span></span>
<span id="cb1-25"><a href="#cb1-25" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">engine</span><span class="kw">:</span><span class="at"> sqlite</span></span>
<span id="cb1-26"><a href="#cb1-26" aria-hidden="true" tabindex="-1"></a><span class="at">    </span><span class="fu">codegen</span><span class="kw">:</span></span>
<span id="cb1-27"><a href="#cb1-27" aria-hidden="true" tabindex="-1"></a><span class="at">      </span><span class="kw">-</span><span class="at"> </span><span class="fu">plugin</span><span class="kw">:</span><span class="at"> csharp</span></span>
<span id="cb1-28"><a href="#cb1-28" aria-hidden="true" tabindex="-1"></a><span class="at">        </span><span class="fu">out</span><span class="kw">:</span><span class="at"> SqliteDalGen</span></span></code></pre></div>
<h1 id="contributing">Contributing</h1>
<h2 id="local-plugin-development">Local plugin development</h2>
<h3 id="prerequisites">Prerequisites</h3>
<p>Make sure that the following applications are installed and added to
your path.</p>
<p>Follow the instructions in each of these: - Dotnet CLI - <a
href="https://github.com/dotnet/sdk">Dotnet Installation</a> - use
version <code>.NET 8.0 (latest)</code> - buf build - <a
href="https://buf.build/docs/installation">Buf Build</a> - WASM (follow
this guide) - <a
href="https://www.strathweb.com/2023/09/dotnet-wasi-applications-in-net-8-0/">WASM
libs</a></p>
<h3 id="protobuf">Protobuf</h3>
<p>SQLC protobuf are defined in sqlc-dev/sqlc repository. Generating C#
code from protocol buffer files:</p>
<pre><code>make protobuf-generate</code></pre>
<h3 id="generating-code">Generating code</h3>
<p>SQLC utilizes our process / WASM plugin to generate code:</p>
<pre><code>make sqlc-generate-process
make sqlc-generate-wasm</code></pre>
<h3 id="testing-generated-code">Testing generated code</h3>
<p>Testing the SQLC generated code via a predefined flow:</p>
<pre><code>make test-process-plugin
make test-wasm-plugin</code></pre>
<h2 id="release-flow">Release flow</h2>
<p>The release flow in this repo follows the semver conventions,
building tag as <code>v[major].[minor].[patch]</code>. In order to
create a release you need to add <code>[release]</code> somewhere in
your commit message when merging to master.</p>
<h3 id="version-bumping-built-on-tags">Version bumping (built on
tags)</h3>
<p>By default, the release script will bump the patch version. Adding
<code>[release]</code> to your commit message results in a new tag with
<code>v[major].[minor].[patch]+1</code>. - Bump <code>minor</code>
version by adding <code>[minor]</code> to your commit message resulting
in a new tag with <code>v[major].[minor]+1.0</code><br/> - Bump
<code>major</code> version by adding <code>[major]</code> to your commit
message resulting in a new tag with <code>v[major]+1.0.0</code></p>
<h3 id="release-structure">Release structure</h3>
<p>The new created tag will create a draft release with it, in the
release there will be the wasm plugin embedded in the release.<br/></p>
<h1 id="examples">Examples</h1>
<h2 id="engine-postgresql-npgsqlexample">Engine <code>postgresql</code>:
<a href="../examples/NpgsqlExample">NpgsqlExample</a></h2>
<h3 id="schema-queries-end2end-test"><a
href="../examples/config/postgresql/schema.sql">Schema</a> | <a
href="../examples/config/postgresql/query.sql">Queries</a> | <a
href="../EndToEndTests/NpgsqlTester.cs">End2End Test</a></h3>
<h3 id="config">Config</h3>
<div class="sourceCode" id="cb5"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb5-1"><a href="#cb5-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">false</span></span>
<span id="cb5-2"><a href="#cb5-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> net8.0</span></span>
<span id="cb5-3"><a href="#cb5-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb5-4"><a href="#cb5-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> NpgsqlExampleGen</span></span></code></pre></div>
<h2 id="engine-postgresql-npgsqldapperexample">Engine
<code>postgresql</code>: <a
href="../examples/NpgsqlDapperExample">NpgsqlDapperExample</a></h2>
<h3 id="schema-queries-end2end-test-1"><a
href="../examples/config/postgresql/schema.sql">Schema</a> | <a
href="../examples/config/postgresql/query.sql">Queries</a> | <a
href="../EndToEndTests/NpgsqlDapperTester.cs">End2End Test</a></h3>
<h3 id="config-1">Config</h3>
<div class="sourceCode" id="cb6"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb6-1"><a href="#cb6-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb6-2"><a href="#cb6-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> net8.0</span></span>
<span id="cb6-3"><a href="#cb6-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb6-4"><a href="#cb6-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> NpgsqlDapperExampleGen</span></span></code></pre></div>
<h2 id="engine-postgresql-npgsqllegacyexample">Engine
<code>postgresql</code>: <a
href="../examples/NpgsqlLegacyExample">NpgsqlLegacyExample</a></h2>
<h3 id="schema-queries-end2end-test-2"><a
href="../examples/config/postgresql/schema.sql">Schema</a> | <a
href="../examples/config/postgresql/query.sql">Queries</a> | <a
href="../LegacyEndToEndTests/NpgsqlTester.cs">End2End Test</a></h3>
<h3 id="config-2">Config</h3>
<div class="sourceCode" id="cb7"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb7-1"><a href="#cb7-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">false</span></span>
<span id="cb7-2"><a href="#cb7-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> netstandard2.0</span></span>
<span id="cb7-3"><a href="#cb7-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb7-4"><a href="#cb7-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> NpgsqlLegacyExampleGen</span></span></code></pre></div>
<h2 id="engine-postgresql-npgsqldapperlegacyexample">Engine
<code>postgresql</code>: <a
href="../examples/NpgsqlDapperLegacyExample">NpgsqlDapperLegacyExample</a></h2>
<h3 id="schema-queries-end2end-test-3"><a
href="../examples/config/postgresql/schema.sql">Schema</a> | <a
href="../examples/config/postgresql/query.sql">Queries</a> | <a
href="../LegacyEndToEndTests/NpgsqlDapperTester.cs">End2End
Test</a></h3>
<h3 id="config-3">Config</h3>
<div class="sourceCode" id="cb8"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb8-1"><a href="#cb8-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb8-2"><a href="#cb8-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> netstandard2.0</span></span>
<span id="cb8-3"><a href="#cb8-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb8-4"><a href="#cb8-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> NpgsqlDapperLegacyExampleGen</span></span></code></pre></div>
<h2 id="engine-mysql-mysqlconnectorexample">Engine <code>mysql</code>:
<a
href="../examples/MySqlConnectorExample">MySqlConnectorExample</a></h2>
<h3 id="schema-queries-end2end-test-4"><a
href="../examples/config/mysql/schema.sql">Schema</a> | <a
href="../examples/config/mysql/query.sql">Queries</a> | <a
href="../EndToEndTests/MySqlConnectorTester.cs">End2End Test</a></h3>
<h3 id="config-4">Config</h3>
<div class="sourceCode" id="cb9"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb9-1"><a href="#cb9-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">false</span></span>
<span id="cb9-2"><a href="#cb9-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> net8.0</span></span>
<span id="cb9-3"><a href="#cb9-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb9-4"><a href="#cb9-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> MySqlConnectorExampleGen</span></span></code></pre></div>
<h2 id="engine-mysql-mysqlconnectordapperexample">Engine
<code>mysql</code>: <a
href="../examples/MySqlConnectorDapperExample">MySqlConnectorDapperExample</a></h2>
<h3 id="schema-queries-end2end-test-5"><a
href="../examples/config/mysql/schema.sql">Schema</a> | <a
href="../examples/config/mysql/query.sql">Queries</a> | <a
href="../EndToEndTests/MySqlConnectorDapperTester.cs">End2End
Test</a></h3>
<h3 id="config-5">Config</h3>
<div class="sourceCode" id="cb10"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb10-1"><a href="#cb10-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb10-2"><a href="#cb10-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> net8.0</span></span>
<span id="cb10-3"><a href="#cb10-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb10-4"><a href="#cb10-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> MySqlConnectorDapperExampleGen</span></span></code></pre></div>
<h2 id="engine-mysql-mysqlconnectorlegacyexample">Engine
<code>mysql</code>: <a
href="../examples/MySqlConnectorLegacyExample">MySqlConnectorLegacyExample</a></h2>
<h3 id="schema-queries-end2end-test-6"><a
href="../examples/config/mysql/schema.sql">Schema</a> | <a
href="../examples/config/mysql/query.sql">Queries</a> | <a
href="../LegacyEndToEndTests/MySqlConnectorTester.cs">End2End
Test</a></h3>
<h3 id="config-6">Config</h3>
<div class="sourceCode" id="cb11"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb11-1"><a href="#cb11-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">false</span></span>
<span id="cb11-2"><a href="#cb11-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> netstandard2.0</span></span>
<span id="cb11-3"><a href="#cb11-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb11-4"><a href="#cb11-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> MySqlConnectorLegacyExampleGen</span></span></code></pre></div>
<h2 id="engine-mysql-mysqlconnectordapperlegacyexample">Engine
<code>mysql</code>: <a
href="../examples/MySqlConnectorDapperLegacyExample">MySqlConnectorDapperLegacyExample</a></h2>
<h3 id="schema-queries-end2end-test-7"><a
href="../examples/config/mysql/schema.sql">Schema</a> | <a
href="../examples/config/mysql/query.sql">Queries</a> | <a
href="../LegacyEndToEndTests/MySqlConnectorDapperTester.cs">End2End
Test</a></h3>
<h3 id="config-7">Config</h3>
<div class="sourceCode" id="cb12"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb12-1"><a href="#cb12-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb12-2"><a href="#cb12-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> netstandard2.0</span></span>
<span id="cb12-3"><a href="#cb12-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb12-4"><a href="#cb12-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> MySqlConnectorDapperLegacyExampleGen</span></span></code></pre></div>
<h2 id="engine-sqlite-sqliteexample">Engine <code>sqlite</code>: <a
href="../examples/SqliteExample">SqliteExample</a></h2>
<h3 id="schema-queries-end2end-test-8"><a
href="../examples/config/sqlite/schema.sql">Schema</a> | <a
href="../examples/config/sqlite/query.sql">Queries</a> | <a
href="../EndToEndTests/SqliteTester.cs">End2End Test</a></h3>
<h3 id="config-8">Config</h3>
<div class="sourceCode" id="cb13"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb13-1"><a href="#cb13-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">false</span></span>
<span id="cb13-2"><a href="#cb13-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> net8.0</span></span>
<span id="cb13-3"><a href="#cb13-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb13-4"><a href="#cb13-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> SqliteExampleGen</span></span></code></pre></div>
<h2 id="engine-sqlite-sqlitedapperexample">Engine <code>sqlite</code>:
<a href="../examples/SqliteDapperExample">SqliteDapperExample</a></h2>
<h3 id="schema-queries-end2end-test-9"><a
href="../examples/config/sqlite/schema.sql">Schema</a> | <a
href="../examples/config/sqlite/query.sql">Queries</a> | <a
href="../EndToEndTests/SqliteDapperTester.cs">End2End Test</a></h3>
<h3 id="config-9">Config</h3>
<div class="sourceCode" id="cb14"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb14-1"><a href="#cb14-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb14-2"><a href="#cb14-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> net8.0</span></span>
<span id="cb14-3"><a href="#cb14-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb14-4"><a href="#cb14-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> SqliteDapperExampleGen</span></span></code></pre></div>
<h2 id="engine-sqlite-sqlitelegacyexample">Engine <code>sqlite</code>:
<a href="../examples/SqliteLegacyExample">SqliteLegacyExample</a></h2>
<h3 id="schema-queries-end2end-test-10"><a
href="../examples/config/sqlite/schema.sql">Schema</a> | <a
href="../examples/config/sqlite/query.sql">Queries</a> | <a
href="../LegacyEndToEndTests/SqliteTester.cs">End2End Test</a></h3>
<h3 id="config-10">Config</h3>
<div class="sourceCode" id="cb15"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb15-1"><a href="#cb15-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">false</span></span>
<span id="cb15-2"><a href="#cb15-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> netstandard2.0</span></span>
<span id="cb15-3"><a href="#cb15-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb15-4"><a href="#cb15-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> SqliteLegacyExampleGen</span></span></code></pre></div>
<h2 id="engine-sqlite-sqlitedapperlegacyexample">Engine
<code>sqlite</code>: <a
href="../examples/SqliteDapperLegacyExample">SqliteDapperLegacyExample</a></h2>
<h3 id="schema-queries-end2end-test-11"><a
href="../examples/config/sqlite/schema.sql">Schema</a> | <a
href="../examples/config/sqlite/query.sql">Queries</a> | <a
href="../LegacyEndToEndTests/SqliteDapperTester.cs">End2End
Test</a></h3>
<h3 id="config-11">Config</h3>
<div class="sourceCode" id="cb16"><pre
class="sourceCode yaml"><code class="sourceCode yaml"><span id="cb16-1"><a href="#cb16-1" aria-hidden="true" tabindex="-1"></a><span class="fu">useDapper</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb16-2"><a href="#cb16-2" aria-hidden="true" tabindex="-1"></a><span class="fu">targetFramework</span><span class="kw">:</span><span class="at"> netstandard2.0</span></span>
<span id="cb16-3"><a href="#cb16-3" aria-hidden="true" tabindex="-1"></a><span class="fu">generateCsproj</span><span class="kw">:</span><span class="at"> </span><span class="ch">true</span></span>
<span id="cb16-4"><a href="#cb16-4" aria-hidden="true" tabindex="-1"></a><span class="fu">namespaceName</span><span class="kw">:</span><span class="at"> SqliteDapperLegacyExampleGen</span></span></code></pre></div>
